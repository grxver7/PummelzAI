using UnityEngine;

public static class MGUtils
{

    //position must be center position for this to work
    public static void keepWithinBounds(GameObject gObject, Vector3 position, Transform boundTransform)
    {
        float height = Screen.height;
        float width = Screen.width;

        //Debug.LogError("screen " + Screen.width + "*" + Screen.height);

        float minX = 0;
        float minY = 0;
        float maxX = width;
        float maxY = height;


        float newX = Camera.main.WorldToScreenPoint(position).x;
        float newY = Camera.main.WorldToScreenPoint(position).y;

      


        //keep tooltip fully visible to camera
        Vector3 extents = new Vector3(144.0f, 200.0f, 0.0f);

        Vector3[] worldCorners = new Vector3[4];
        gObject.GetComponent<RectTransform>().GetWorldCorners(worldCorners); //new Vector3(  , 200.0f, 0.0f);


        float lengthX = Mathf.Abs(Camera.main.WorldToScreenPoint(worldCorners[0]).x - Camera.main.WorldToScreenPoint(worldCorners[2]).x);
        float lengthY = Mathf.Abs(Camera.main.WorldToScreenPoint(worldCorners[0]).y - Camera.main.WorldToScreenPoint(worldCorners[1]).y);

        extents = new Vector3(lengthX / 2.0f, lengthY / 2.0f, 0f);

        if (newX + extents.x > maxX)
        {
            newX = maxX - extents.x;
        }
        else if (newX - extents.x < minX)
        {
            newX = minX + extents.x;
        }

        if (newY + extents.y > maxY)
        {
            newY = maxY - extents.y;
        }
        else if (newY - extents.y < minY)
        {
            newY = minY + extents.y;
        }
   
        gObject.transform.position = new Vector3(newX, newY, gObject.transform.position.z);
       


    }
}
