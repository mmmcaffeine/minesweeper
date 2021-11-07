﻿using System;
using System.Runtime.Serialization;
using System.Text;

namespace Dgt.Minesweeper.Engine
{
    [Serializable]
    public class InvalidMoveException : Exception
    {
        private const string DefaultMessage = "The move is not valid due to the current state of the Cell at the given Location.";
        
        #region Recommended constructors
        
        // I'm not sure if I want to keep these. This exception doesn't really make sense without the Cell and Location
        // properties being set. However, Microsoft best practices recommend having these, and they are based off code
        // that is generated by Visual Studio when generating a custom exception class.
        //
        // See https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions
        // See https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions

        public InvalidMoveException()
            : this(DefaultMessage)
        {
        }

        public InvalidMoveException(string message)
            : base(message)
        {
        }

        public InvalidMoveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        protected InvalidMoveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        #endregion

        public InvalidMoveException(Cell cell, string state, string reason)
            : this(CreateMessage(cell.Location, state, reason))
        {
            Cell = cell;
        }

        private static string CreateMessage(Location location, string state, string explanation)
        {
            var messageBuilder = new StringBuilder(DefaultMessage);

            messageBuilder.Append($" The Cell at Location \"{(string)location}\" has been {state}.");
            messageBuilder.Append($" {explanation}.");

            return messageBuilder.ToString();
        }
        
        public Cell? Cell { get; }
        public Location? Location => Cell?.Location;
    }
}