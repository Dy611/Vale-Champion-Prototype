using UnityEngine;

public static class StandardFunctions
{
    public static float CalculateDistance(Vector3 destination, Vector3 source)
    {
        float xDiff = destination.x - source.x;
        float zDiff = destination.z - source.z;

        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }

    public static void RotateObj(GameObject obj, Vector3 destination)
    {
        Vector3 lookDir = destination - obj.transform.position;
        lookDir.y = 0;
        obj.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    public static RaycastHit GetMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        return hit;
    }

    public static RaycastHit GetMousePosition(LayerMask layers)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 1000f, layers);
        return hit;
    }
}