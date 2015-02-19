namespace ApacsAdapter
{
    public class AdpCardHolder
    {
        public string Photo { get; set; }
        public string HolderID { get; set; }
        public string HolderName { get; set; }      
        public string HolderShortName { get; set; }
        public uint CardNo { get; set; }
        public override string ToString()
        {
            return "=====================BEGIN "+ HolderShortName +"=====================================\n"
                 + "Photo: " + Photo + "\n"
                 + "HolderID: " + HolderID + "\n"
                 + "HolderName: " + HolderName + "\n"
                 + "HolderShortName: " + HolderShortName + "\n"
                 + "CardNo: " + CardNo.ToString() + "\n"
                 + "=====================END " + HolderShortName + "=====================================\n";
        }
    }
}
