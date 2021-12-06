using UnityEngine;

namespace Nexus.Utilities
{
    public class Utils
    {
        public static string ColourString(Color col, string inString)
        {
            string colString = ColorUtility.ToHtmlStringRGB(col);
            return Utils.ColourString("#" + colString, inString);
        }

        public static string ColourString(string colString, string inString)
        {
            string outString = "<color=" + colString + ">" + inString + "</color>";
            return outString;
        }

        public static double AngleDifference(double angle1, double angle2)
        {
            double diff = (angle2 - angle1 + 180) % 360 - 180;
            return diff < -180 ? diff + 360 : diff;
        }

        public static float GetAngleFromXY(Vector2 inVec)
        {
            return Mathf.Atan2(inVec.x, inVec.y);
        }
    }
}
