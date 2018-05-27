using System.Collections.Generic;

namespace DataModel
{
    public class ConnectionData
    {
        /*
         * 更新字段，要更新 ConnectionDataValueSetExtension.cs 文件
         */
        public const string C_DIRECTION = "Direction";
        public const string C_COMMAND = "Command";
        public const string C_DATA = "Data";

        public Direction Direction { get; set; } = Direction.None;
        public Command Command { get; set; } = Command.None;
        public ExChangeData Data { get; set; } = null;

        public int Integer { get; set; }
        public bool Boolean { get; set; }
        public string Text { get; set; }

        public ConnectionData()
        {

        }

        public ConnectionData(Direction direct, Command cmd)
        {
            Command = cmd;
            Direction = direct;
        }

        public ConnectionData(Direction direct, Command cmd, ExChangeData data)
        {
            Command = cmd;
            Direction = direct;
            Data = data;
        }

        public override string ToString()
        {
            return $"{Direction}: {Command}";
        }
    }

}
