﻿using CommonLibrary;
using System;
using Windows.Foundation.Collections;

namespace DataModel
{
    public static class ValueSetExtension
    {
        public static ValueSet ToValueSet(this ConnectionData data)
        {
            var set = new ValueSet
            {
                { ConnectionData.C_DIRECTION, data.Direction.ToString() },
                { ConnectionData.C_COMMAND, data.Command.ToString() },
                { ConnectionData.C_DATA, data.Data.ToXmlText() },
                { "Integer", data.Integer },
                { "Boolean", data.Boolean },
                { "Text", data.Text },
            };
            return set;
        }

        public static ConnectionData ToConnectionData(this ValueSet set)
        {
            if (set.ContainsKey(ConnectionData.C_DIRECTION))
            {
                var dir = (Direction)Enum.Parse(typeof(Direction), set[ConnectionData.C_DIRECTION] as string);
                var req = (Command)Enum.Parse(typeof(Command), set[ConnectionData.C_COMMAND] as string);
                var dat = XmlSerialization.FromXmlText<ExChangeData>(set[ConnectionData.C_DATA] as string);
                return new ConnectionData
                {
                    Direction = dir,
                    Command = req,
                    Data = dat,
                    Integer = (int)set["Integer"],
                    Boolean = (bool)set["Boolean"],
                    Text = set["Text"] as string,
                };
            }
            else
            {
                return new ConnectionData();
            }
        }
    }
}
