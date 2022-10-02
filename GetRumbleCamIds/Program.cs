using SoulsAssetPipeline.Animation;
using SoulsFormats;

namespace GetRumbleCamIds
{
    class Program
    {
        public static void Main(string[] args)
        {
            string dir = args.FirstOrDefault(x => Directory.Exists(x), "");
            if (dir.Equals("") == false)
            {
                List<short> rumbleCamIds = new();
                IEnumerable<string> anibndPaths = Directory.EnumerateFiles(dir, "*.anibnd.dcx");
                foreach (var path in anibndPaths)
                {
                    BND4 bnd = BND4.Read(path);
                    IEnumerable<BinderFile> taeFiles = bnd.Files.Where(x => x.Name.Contains(".tae"));
                    foreach (var taeFile in taeFiles.Where(x => x.Bytes.Length > 0))
                    {
                        TAE tae = TAE.Read(taeFile.Bytes);
                        foreach (var anim in tae.Animations)
                        {
                            foreach (var ev in anim.Events)
                            {
                                if (ev.Type is >= 144 and <= 147)
                                {
                                    byte[] paramBytes = ev.GetParameterBytes(tae.BigEndian);
                                    short rumbleCamId = BitConverter.ToInt16(paramBytes, 0);
                                    rumbleCamIds.Add(rumbleCamId);
                                }
                            }
                        }
                    }
                }
                rumbleCamIds = rumbleCamIds.Distinct().OrderBy(x => x).ToList();
                File.WriteAllLines($"{AppDomain.CurrentDomain.BaseDirectory}EveryUsedRumbleCamId.txt", rumbleCamIds.Select(x=>x.ToString()));
            }
        }
    }
}

