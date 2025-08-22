using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace PF.Contracts
{
    /// <summary>
    /// Identifies a logging event. The primary identifier is the "Id" property, with the "Name" property providing a short description of this type of event.
    /// </summary>
    public readonly struct LogEventId : IEquatable<LogEventId>
    {
        /// <summary>
        /// Implicitly creates an EventId from the given <see cref="int"/>.
        /// </summary>
        /// <param name="i">The <see cref="int"/> to convert to an EventId.</param>
        public static implicit operator LogEventId(int i)
        {
            return new LogEventId(i);
        }

        /// <summary>
        /// Checks if two specified <see cref="LogEventId"/> instances have the same value. They are equal if they have the same ID.
        /// </summary>
        /// <param name="left">The first <see cref="LogEventId"/>.</param>
        /// <param name="right">The second <see cref="LogEventId"/>.</param>
        /// <returns><see langword="true" /> if the objects are equal.</returns>
        public static bool operator ==(LogEventId left, LogEventId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks if two specified <see cref="LogEventId"/> instances have different values.
        /// </summary>
        /// <param name="left">The first <see cref="LogEventId"/>.</param>
        /// <param name="right">The second <see cref="LogEventId"/>.</param>
        /// <returns><see langword="true" /> if the objects are not equal.</returns>
        public static bool operator !=(LogEventId left, LogEventId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes an instance of the <see cref="LogEventId"/> struct.
        /// </summary>
        /// <param name="id">The numeric identifier for this event.</param>
        /// <param name="name">The name of this event.</param>
        public LogEventId(int id, string? name = null)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the numeric identifier for this event.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the name of this event.
        /// </summary>
        public string? Name { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name ?? Id.ToString();
        }

        /// <summary>
        /// Compares the current instance to another object of the same type. Two events are equal if they have the same ID.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to <paramref name="other" />; otherwise, <see langword="false" />.</returns>
        public bool Equals(LogEventId other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is LogEventId eventId && Equals(eventId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id;
        }
    }
}