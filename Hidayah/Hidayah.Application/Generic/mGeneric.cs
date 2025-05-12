namespace Hidayah.Application.Generic
{
    public class mGeneric
    {
        public class mApiResponse<T>
        {
            public int RespCode { get; set; }
            public string RespMsg { get; set; }
            public T RespData { get; set; }

            public mApiResponse() { }

            public mApiResponse(int respCode, string respMsg, T respData)
            {
                RespCode = respCode;
                RespMsg = respMsg;
                RespData = respData;
            }

            public mApiResponse(int respCode, string respMsg)
            {
                RespCode = respCode;
                RespMsg = respMsg;
            }

            public mApiResponse(string respMsg, T respData)
            {
                RespMsg = respMsg;
                RespData = respData;
            }
        }
    }

}
