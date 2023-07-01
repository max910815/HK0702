using System;

namespace HK_Product.ViewModels
{
    public class QagptViewModel
    {
        public string ChatId { get; set; }
        public DateTime ChatTime { get; set; }
        public string ChatName { get; set; }

        public string QahistoryId { get; set; }
        public string QahistoryQ { get; set; }
        public string QahistoryA { get; set; }
        public string QahistoryVectors { get; set; }

        public string ApplicationId { get; set; }
        public string AppModel { get; set; }
        public string AppParameter { get; set; }
    }
}
