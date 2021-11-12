using System;

namespace GirinTest.Exceptions
{
    public class FailDeserializationException : Exception
    {
        public override string Message { get; } = "Ошибка выполнения десериализации. Некорректные данные";

        public FailDeserializationException()
        {
        }
    }
}