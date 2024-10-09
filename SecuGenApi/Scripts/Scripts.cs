using System.Data.SqlTypes;
using System.Security.Cryptography;

namespace SecuGenApi.Scripts
{
    public class Scripts
    {
        public string GetFPRow()
        {
            string sqlString = @"Select * from fingerprints";
            return sqlString;

        }
    }
}


