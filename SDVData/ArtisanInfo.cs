namespace SDVData
{
    public class ArtisanInfo
    {
        public string MachineId { get; set; } = ""; //qualified ID e.g. (BC)17
        public string MachineName { get; set; } = ""; //e.g. Loom

        public string InputId { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        //TODO Input by category e.g. smoker
        public string InputName { get; set; } = "";
        public int InputCount { get; set; }


        public string OutputId { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        public string OutputName { get; set; } = "";
        public int OutputCount { get; set; }
        //TODO Output quality

        //TODO Fuel: e.g. coal in smoker

        //public List<MachineFuel> Fuel { get; set; } = new(); //e.g. coal in smoker
    }

    public class MachineFuel
    {
        public string FuelId { get; set; } = ""; //unqualified ID (do not have (O) prefix)
        public int Count { get; set; }
    }
}
