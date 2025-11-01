namespace QuizEngineBE.DTO.PullSeedSpace
{
    public class Modes
    {
        public static readonly string[] List = ["sfida", "verifica"];

        public static bool Contains(string mode)
                => List.Contains(mode);
    }
}
