using System.Collections.Generic;

namespace DataModel
{
    public class ConnectionData
    {
        /*
         * 更新字段，要更新 ConnectionDataValueSetExtension.cs 文件
         */
        public const string C_Type = "Type";
        public const string C_COMMAND = "Command";
        public const string C_DATA = "Data";

        public MsgType Type { get; set; } = MsgType.None;
        public Command Command { get; set; } = Command.None;
        public ExChangeData Data { get; set; } = null;

        public int Integer { get; set; }
        public bool Boolean { get; set; }
        public string Text { get; set; }

        public ConnectionData()
        {

        }

        public ConnectionData(MsgType type, Command cmd)
        {
            Type = type;
            Command = cmd;
        }

        public ConnectionData(MsgType type, Command cmd, ExChangeData data)
        {
            Type = type;
            Command = cmd;
            Data = data;
        }

        public override string ToString()
        {
            return $"{Type}: {Command}, ({Boolean}, {Integer}, {Text})";
        }
    }

}
