// Decompiled with JetBrains decompiler
// Type: YawGEAPI.Quaternion
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System;


namespace YawGEAPI
{
    /// <summary>
    /// Represents a quaternion in 3D space.
    /// </summary>
    public class Quaternion
    {
        /// <summary>
        /// Gets or sets the x-component of the quaternion.
        /// </summary>
        public double x;

        /// <summary>
        /// Gets or sets the y-component of the quaternion.
        /// </summary>
        public double y;

        /// <summary>
        /// Gets or sets the z-component of the quaternion.
        /// </summary>
        public double z;

        /// <summary>
        /// Gets or sets the w-component of the quaternion.
        /// </summary>
        public double w;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> class.
        /// </summary>
        public Quaternion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> class with the specified components.
        /// </summary>
        /// <param name="x">The x-component of the quaternion.</param>
        /// <param name="y">The y-component of the quaternion.</param>
        /// <param name="z">The z-component of the quaternion.</param>
        /// <param name="w">The w-component of the quaternion.</param>
        public Quaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Calculates the pitch angle in radians from a Y-up quaternion.
        /// </summary>
        /// <returns>The pitch angle in radians.</returns>
        public double toPitchFromYUp()
        {
            double num1 = 2.0 * (this.x * this.y + this.w * this.y);
            double y = 2.0 * (this.w * this.x - this.y * this.z);
            double num2 = 1.0 - 2.0 * (this.x * this.x + this.y * this.y);
            return Math.Atan2(y, Math.Sqrt(num1 * num1 + num2 * num2));
        }

        /// <summary>
        /// Calculates the yaw angle in radians from a Y-up quaternion.
        /// </summary>
        /// <returns>The yaw angle in radians.</returns>
        public double toYawFromYUp()
        {
            return Math.Atan2(2.0 * (this.x * this.y + this.w * this.y), 1.0 - 2.0 * (this.x * this.x + this.y * this.y));
        }

        /// <summary>
        /// Calculates the roll angle in radians from a Y-up quaternion.
        /// </summary>
        /// <returns>The roll angle in radians.</returns>
        public double toRollFromYUp()
        {
            return Math.Atan2(2.0 * (this.x * this.y + this.w * this.z), 1.0 - 2.0 * (this.x * this.x + this.z * this.z));
        }
    }
}
