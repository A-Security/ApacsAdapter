namespace ApacsAdapter
{
    public class AdpCardHolder
    {
        public byte[] Photo { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }      
        public string ShortName { get; set; }
        public uint CardNo { get; set; }
        public override string ToString()
        {
            return "=====================BEGIN "+ ShortName +"=====================================\n"
                 + "Photo: " + Photo + "\n"
                 + "ID: " + ID + "\n"
                 + "Name: " + Name + "\n"
                 + "ShortName: " + ShortName + "\n"
                 + "CardNo: " + CardNo.ToString() + "\n"
                 + "=====================END " + ShortName + "=====================================\n";
        }
    }
}
