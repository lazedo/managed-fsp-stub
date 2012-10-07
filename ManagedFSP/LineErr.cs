using System;
using System.Collections.Generic;
using System.Text;

namespace ManagedFSP
{
    public enum LineErr : uint
    {
          LINEERR_ALLOCATED                       = 0x80000001
        , LINEERR_BADDEVICEID                     = 0x80000002
        , LINEERR_BEARERMODEUNAVAIL               = 0x80000003
        , LINEERR_CALLUNAVAIL                     = 0x80000005
        , LINEERR_COMPLETIONOVERRUN               = 0x80000006
        , LINEERR_CONFERENCEFULL                  = 0x80000007
        , LINEERR_DIALBILLING                     = 0x80000008
        , LINEERR_DIALDIALTONE                    = 0x80000009
        , LINEERR_DIALPROMPT                      = 0x8000000A
        , LINEERR_DIALQUIET                       = 0x8000000B
        , LINEERR_INCOMPATIBLEAPIVERSION          = 0x8000000C
        , LINEERR_INCOMPATIBLEEXTVERSION          = 0x8000000D
        , LINEERR_INIFILECORRUPT                  = 0x8000000E
        , LINEERR_INUSE                           = 0x8000000F
        , LINEERR_INVALADDRESS                    = 0x80000010
        , LINEERR_INVALADDRESSID                  = 0x80000011
        , LINEERR_INVALADDRESSMODE                = 0x80000012
        , LINEERR_INVALADDRESSSTATE               = 0x80000013
        , LINEERR_INVALAPPHANDLE                  = 0x80000014
        , LINEERR_INVALAPPNAME                    = 0x80000015
        , LINEERR_INVALBEARERMODE                 = 0x80000016
        , LINEERR_INVALCALLCOMPLMODE              = 0x80000017
        , LINEERR_INVALCALLHANDLE                 = 0x80000018
        , LINEERR_INVALCALLPARAMS                 = 0x80000019
        , LINEERR_INVALCALLPRIVILEGE              = 0x8000001A
        , LINEERR_INVALCALLSELECT                 = 0x8000001B
        , LINEERR_INVALCALLSTATE                  = 0x8000001C
        , LINEERR_INVALCALLSTATELIST              = 0x8000001D
        , LINEERR_INVALCARD                       = 0x8000001E
        , LINEERR_INVALCOMPLETIONID               = 0x8000001F
        , LINEERR_INVALCONFCALLHANDLE             = 0x80000020
        , LINEERR_INVALCONSULTCALLHANDLE          = 0x80000021
        , LINEERR_INVALCOUNTRYCODE                = 0x80000022
        , LINEERR_INVALDEVICECLASS                = 0x80000023
        , LINEERR_INVALDEVICEHANDLE               = 0x80000024
        , LINEERR_INVALDIALPARAMS                 = 0x80000025
        , LINEERR_INVALDIGITLIST                  = 0x80000026
        , LINEERR_INVALDIGITMODE                  = 0x80000027
        , LINEERR_INVALDIGITS                     = 0x80000028
        , LINEERR_INVALEXTVERSION                 = 0x80000029
        , LINEERR_INVALGROUPID                    = 0x8000002A
        , LINEERR_INVALLINEHANDLE                 = 0x8000002B
        , LINEERR_INVALLINESTATE                  = 0x8000002C
        , LINEERR_INVALLOCATION                   = 0x8000002D
        , LINEERR_INVALMEDIALIST                  = 0x8000002E
        , LINEERR_INVALMEDIAMODE                  = 0x8000002F
        , LINEERR_INVALMESSAGEID                  = 0x80000030
        , LINEERR_INVALPARAM                      = 0x80000032
        , LINEERR_INVALPARKID                     = 0x80000033
        , LINEERR_INVALPARKMODE                   = 0x80000034
        , LINEERR_INVALPOINTER                    = 0x80000035
        , LINEERR_INVALPRIVSELECT                 = 0x80000036
        , LINEERR_INVALRATE                       = 0x80000037
        , LINEERR_INVALREQUESTMODE                = 0x80000038
        , LINEERR_INVALTERMINALID                 = 0x80000039
        , LINEERR_INVALTERMINALMODE               = 0x8000003A
        , LINEERR_INVALTIMEOUT                    = 0x8000003B
        , LINEERR_INVALTONE                       = 0x8000003C
        , LINEERR_INVALTONELIST                   = 0x8000003D
        , LINEERR_INVALTONEMODE                   = 0x8000003E
        , LINEERR_INVALTRANSFERMODE               = 0x8000003F
        , LINEERR_LINEMAPPERFAILED                = 0x80000040
        , LINEERR_NOCONFERENCE                    = 0x80000041
        , LINEERR_NODEVICE                        = 0x80000042
        , LINEERR_NODRIVER                        = 0x80000043
        , LINEERR_NOMEM                           = 0x80000044
        , LINEERR_NOREQUEST                       = 0x80000045
        , LINEERR_NOTOWNER                        = 0x80000046
        , LINEERR_NOTREGISTERED                   = 0x80000047
        , LINEERR_OPERATIONFAILED                 = 0x80000048
        , LINEERR_OPERATIONUNAVAIL                = 0x80000049
        , LINEERR_RATEUNAVAIL                     = 0x8000004A
        , LINEERR_RESOURCEUNAVAIL                 = 0x8000004B
        , LINEERR_REQUESTOVERRUN                  = 0x8000004C
        , LINEERR_STRUCTURETOOSMALL               = 0x8000004D
        , LINEERR_TARGETNOTFOUND                  = 0x8000004E
        , LINEERR_TARGETSELF                      = 0x8000004F
        , LINEERR_UNINITIALIZED                   = 0x80000050
        , LINEERR_USERUSERINFOTOOBIG              = 0x80000051
        , LINEERR_REINIT                          = 0x80000052
        , LINEERR_ADDRESSBLOCKED                  = 0x80000053
        , LINEERR_BILLINGREJECTED                 = 0x80000054
        , LINEERR_INVALFEATURE                    = 0x80000055
        , LINEERR_NOMULTIPLEINSTANCE              = 0x80000056
        , LINEERR_INVALAGENTID                    = 0x80000057      // TAPI v2.0
        , LINEERR_INVALAGENTGROUP                 = 0x80000058      // TAPI v2.0
        , LINEERR_INVALPASSWORD                   = 0x80000059      // TAPI v2.0
        , LINEERR_INVALAGENTSTATE                 = 0x8000005A      // TAPI v2.0
        , LINEERR_INVALAGENTACTIVITY              = 0x8000005B      // TAPI v2.0
        , LINEERR_DIALVOICEDETECT                 = 0x8000005C      // TAPI v2.0
        , LINEERR_USERCANCELLED                   = 0x8000005D      // TAPI v2.2
        , LINEERR_INVALADDRESSTYPE                = 0x8000005E      // TAPI v3.0
        , LINEERR_INVALAGENTSESSIONSTATE          = 0x8000005F      // TAPI v2.2
        , LINEERR_DISCONNECTED                    = 0x80000060
        , LINEERR_SERVICE_NOT_RUNNING             = 0x80000061
    }
}
