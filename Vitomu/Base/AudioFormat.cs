namespace Vitomu.Base
{
    public class AudioFormat
    {
        #region Properties
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string YoutubedlCodec { get; private set; }
        public string Extension { get; private set; }
        #endregion

        #region Construction
        public AudioFormat(string id, string name, string youtubedlCodec, string extension)
        {
            this.Id = id;
            this.Name = name;
            this.YoutubedlCodec = youtubedlCodec;
            this.Extension = extension;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AudioFormat))
            {
                return false;
            }

            return this.Id.Equals(((AudioFormat)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}
