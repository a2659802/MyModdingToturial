using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ReflectionExam
{
    public abstract class AbstractShape
    {
        public Vector2 position;
        public abstract void Draw();
        public AbstractShape()
        {
            Modding.Logger.Log("An Concrete class has been instantiate");
            Draw();
        }
    }
    public class Shapes
    {
        public class RectShape : AbstractShape
        {
            float width;
            float height;
            public override void Draw()
            {
                Modding.Logger.Log("RectShape Drawn");
            }
        }
        public class CicrleShape : AbstractShape
        {
            float r;

            public override void Draw()
            {
                Modding.Logger.Log("CircleShape Drawn");
            }
        }
    }
}
