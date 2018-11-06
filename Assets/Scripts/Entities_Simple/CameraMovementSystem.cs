using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CameraMovementSystem : ComponentSystem
{
    public Transform cameraTransform; 

    public void FindCameraTransform()
    {
        cameraTransform = Camera.main.transform;
    }

    public struct PlayerGroup
    {
        public ComponentDataArray<PlayerInput> Player;
        public ComponentDataArray<Position> Position;
    }

    [Inject] PlayerGroup player;

    protected override void OnUpdate()
    {
        if (cameraTransform != null)
        {
            cameraTransform.position = player.Position[0].Value + new float3(Bootstrap.Settings.camOffset.x, Bootstrap.Settings.camOffset.y, Bootstrap.Settings.camOffset.z);
            cameraTransform.GetComponent<Camera>().fieldOfView = Bootstrap.Settings.camZoomSize;
        }
    }

}