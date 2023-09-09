using UnityEngine;
using VTS.Core;
using VTS.Unity;
using Live2D.Cubism.Core;

// INFO ----------------------------------------------------------------------------------------------------------------------------------- //
// To calculate the position of the head I actually use the vertex positions in the CubismDrawable component.
// I take the vertex point that has the highest Y value and add the current CubismDrawable Gameobject position to get the world position.
// This might be a dirty way of doing it but that's the only way I got this to work, so feel free to replace it with your own system.
// ---------------------------------------------------------------------------------------------------------------------------------------- //

public class TutelOnHeadPlugin : UnityVTSPlugin
{
    [Header("Debug")]
    [SerializeField][Tooltip("Displays a red dot to indicate the top of the model's head")] private bool displayHeadPos = true;
    [Header("References")]
    [SerializeField][Tooltip("The head artmesh (found in \"Drawables\")")] private CubismDrawable head;
    [SerializeField][Tooltip("The model reference")] private CubismModel model;
    [Header("Settings")]
    [SerializeField][Tooltip("Allows you to offset your Vtuber avatar position in case it's not properly placed on the head")] private Vector2 modelOffset;

    private float modelSize; // Stores the size of the model before moving it
    private bool isConnected; // Used to check if we are connected to the Vtube Studio API
    private bool movementLocked; // This simply prevent this script from moving your Vtuber model (basically a utility thing)

    private int headTopIndex; // Stores the index of the highest point in the CubismDrawable, this is used to track the top of the head
    private Vector3 headAttachPosition; // The position in world space of the top of the head

    #region UNITY METHODS

    private void Reset()
    {
        _pluginName = "Tutel On Head";
        _pluginAuthor = "DayMoniakk";
    }

    private void Start()
    {
        ConnectToVtubeStudio();
    }

    private void LateUpdate()
    {
        if (!isConnected) return; // If we are not connected to Vtube Studio we don't need to execute this part
        if (movementLocked) return; // In case you need to adjust something in Vtube Studio so this script won't override anything

        headAttachPosition = GetAttachPosition(); // Get the current position of the top of the head

        VTSMoveModelData.Data newPos = new VTSMoveModelData.Data() // Fill the request to move the avatar in Vtube Studio
        {
            positionX = headAttachPosition.x + modelOffset.x,
            positionY = headAttachPosition.y + modelOffset.y,
            size = modelSize
        };

        // Actually move the model in Vtube Studio
        MoveModel(newPos,
        (success) => { },
        (error) =>
        {
            Debug.LogError("Cannot move model > " + error.data.message);
        });
    }

    #endregion

    /// <summary>
    /// Tell the script to connect to the Vtube Studio API
    /// </summary>
    public void ConnectToVtubeStudio()
    {
        headTopIndex = GetHighestYIndex(head.VertexPositions); // Calculate the highest point of the head

        Initialize(
            new WebSocketSharpImpl(Logger),
            new NewtonsoftJsonUtilityImpl(),
            new TokenStorageImpl(Application.persistentDataPath),
            () =>
            {
                Logger.Log("Connected!");
                CacheModelScale();

                isConnected = true;
            },
            () =>
            {
                Logger.LogWarning("Disconnected!");
            },
            (error) =>
            {
                Logger.LogError("Error! - " + error.data.message);
            });
    }

    /// <summary>
    /// Tell the script to disconnect from the Vtube Studio API
    /// </summary>
    public void DisconnectFromVtubeStudio()
    {
        Disconnect();
    }

    /// <summary>
    /// Let you activate or disable the movement override of your avatar
    /// <param name="state">False will disable the movement</param>
    /// </summary>
    public void SetMovement(bool state)
    {
        movementLocked = !state;
    }

    #region UTILITY

    private void CacheModelScale() // Cache the current size of the model, if we don't then the size is going to be reverted to the default value
    {
        // Doing it this way has a drawback, you cannot change the scale of your avatar in Vtube Studio while this script is moving it.
        // So you have to set your avatar scale before starting the script

        GetCurrentModel((success) =>
        {
            modelSize = success.data.modelPosition.size;
        },
        (error) =>
        {
            Debug.LogError("Cannot access model size > " + error.data.message);
        });
    }

    private Vector3 GetAttachPosition() // Get the top head position
    {
        return head.transform.position + head.VertexPositions[headTopIndex];
    }

    // Returns the highest Y position of the head vertex positions
    private int GetHighestYIndex(Vector3[] array)
    {
        int highestIndex = 0;
        float highestY = array[0].y;

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i].y > highestY)
            {
                highestY = array[i].y;
                highestIndex = i;
            }
        }

        return highestIndex;
    }

    #endregion

    #region DEBUG

    // Debug stuff
    private void OnDrawGizmos()
    {
        if (!displayHeadPos || !head) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(DebugGetAttachPosition(), 0.01f);
    }

    private Vector3 DebugGetAttachPosition() // The same as "GetAttachPosition" but this can run outside of play mode
    {
        Vector3[] v = head.VertexPositions;
        return head.transform.position + head.VertexPositions[GetHighestYIndex(v)];
    }

    #endregion
}
