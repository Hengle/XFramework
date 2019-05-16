public interface IArchive
{
    void Save(byte[] data, string path);
    byte[] Load(string path);
}