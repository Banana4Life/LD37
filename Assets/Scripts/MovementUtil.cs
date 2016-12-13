using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUtil {

    public static bool doAnimation(Vector3 from, Transform toMove, float toX, float toZ, float animationFrame, float totalTime)
    {
        var newPos = new Vector3(toX, toMove.position.y, toZ);
        var frame = animationFrame / totalTime;
        if (animationFrame < 0) // Delayed
        {
            return true;
        }

        if (animationFrame > totalTime)
        {
            toMove.position = newPos;
            return false;
        }
        else
        {
            toMove.position = from + (newPos - from) * frame;
            return true;
        }
    }
}
