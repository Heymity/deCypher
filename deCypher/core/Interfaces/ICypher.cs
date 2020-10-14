namespace deCypher
{
    interface ICypher<T>
    {
        T Encode();
        T Decode();
    }
}
