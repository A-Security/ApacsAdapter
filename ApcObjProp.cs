namespace ApacsAdapter
{
    // APACS 3000 Object parameters
    public struct ApcObjProp
    {
        // Object name parameter
        public const string strName = "strName";
        // Object description parameter
        public const string strDesc = "strDesc";
        // Object alias parameter
        public const string strAlias = "strAlias";
        // Object create time parameter
        public const string dtCreateTime = "dtCreateTime";
        // Object last modify time parameter
        public const string dtLastModifyTime = "dtLastModifyTime";
        // Object card number parameter (for TApcCardHolderAccess object)
        public const string dwCardNumber = "dwCardNumber";
        // Object child card Object parameter (for TApcCardHolder object)
        public const string SysAddrCard = "SysAddrCard";
        // Object cardholder name parameter (for TApcCardHolderAccess object)
        public const string strHolderName = "strHolderName";
        // Object child cardholder Object parameter (for TApcCardHolderAccess object)
        public const string SysAddrHolder = "SysAddrHolder";
        // Object if event object create in offline mode parameter 
        public const string isOffLineEvent = "isOffLineEvent";
        // Object event number parameter (for events objects)
        public const string SysAddrEventID = "SysAddrEventID";
        // Object time parameter
        public const string dtRealDateTime = "dtRealDateTime";
        // Object register time parameter
        public const string dtRegisterTime = "dtRegisterTime";
        // Object event type id parameter
        public const string strEventTypeID = "strEventTypeID";
        // Object initialize event object parameter
        public const string SysAddrInitObj = "SysAddrInitObj";
        // Object initialize event object name parameter
        public const string strInitObjName = "strInitObjName";
        // Object photo parameter (for TApcCHMainPhoto property object)
        public const string binBufPhoto = "binBufPhoto";
        // Object last cardholder name parameter (for TApcCardHolder object)
        public const string strLastName = "strLastName";
        // Object middle cardholder name parameter (for TApcCardHolder object)
        public const string strMiddleName = "strMiddleName";
        // Object first cardholder name parameter (for TApcCardHolder object)
        public const string strFirstName = "strFirstName";
        // Object cardholder identification parameter (for TApcAccount2HolderLink object)
        public const string SysAddrAccount = "SysAddrAccount";
        // Object cardholder job title object parameter (for TApcCardHolder object)
        public const string SysAddrJobTitle = "SysAddrJobTitle";
        // Object cardholder company object parameter (for TApcCardHolder object)
        public const string SysAddrCompany = "SysAddrCompany";
        // Object cardholder category parameter (for TApcCardHolder object)
        public const string bEmployee = "bEmployee";
    }
}
