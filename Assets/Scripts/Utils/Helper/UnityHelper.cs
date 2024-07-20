using System.Threading.Tasks;
using UnityEngine;

namespace Utils.Helper
{
    public static class UnityHelper
    {
        public static Vector2 SetX(this Vector2 vector, float x)
        {
            return new(x, vector.y);
        }
        
        public static Vector2 SetY(this Vector2 vector, float y)
        {
            return new(vector.x, y);
        }

        public static Vector3 SetX(this Vector3 vector, float x)
        {
            return new(x, vector.y, vector.z);
        }
        
        public static Vector3 SetY(this Vector3 vector, float y)
        {
            return new(vector.x, y, vector.z);
        }
        
        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            return new(vector.x, vector.y, z);
        }

        public static Task ToTask(this AsyncOperation operation)
        {
            var taskSource = new TaskCompletionSource<bool>();
            operation.completed += _ => taskSource.TrySetResult(operation.isDone);
            return taskSource.Task;
        }
    }
}
