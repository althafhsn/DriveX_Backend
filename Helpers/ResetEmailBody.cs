﻿namespace DriveX_Backend.Helpers
{
    public static class ResetEmailBody
    {
        public static string ResetPasswordEmailStringBody(string email , string emailToken)
        {
            return $@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                                            <html dir=""ltr"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""en"">
                                             <head>
                                              <meta charset=""UTF-8"">
                                              <meta content=""width=device-width, initial-scale=1"" name=""viewport"">
                                              <meta name=""x-apple-disable-message-reformatting"">
                                              <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                                              <meta content=""telephone=no"" name=""format-detection"">
                                              <title>New email template 2024-11-20</title> 
                                              <!--[if (mso 16)]>
                                                <style type=""text/css"">
                                                a {{text-decoration: none;}}
                                                </style>
                                                <![endif]--> 
                                              <!--[if gte mso 9]><style>sup {{ font-size: 100% !important; }}</style><![endif]--> 
                                              <!--[if gte mso 9]>
                                            <noscript>
                                                     <xml>
                                                       <o:OfficeDocumentSettings>
                                                       <o:AllowPNG></o:AllowPNG>
                                                       <o:PixelsPerInch>96</o:PixelsPerInch>
                                                       </o:OfficeDocumentSettings>
                                                     </xml>
                                                  </noscript>
                                            <![endif]-->
                                              <style type=""text/css"">
                                            .rollover:hover .rollover-first {{
                                              max-height:0px!important;
                                              display:none!important;
                                            }}
                                            .rollover:hover .rollover-second {{
                                              max-height:none!important;
                                              display:block!important;
                                            }}
                                            .rollover span {{
                                              font-size:0px;
                                            }}
                                            u + .body img ~ div div {{
                                              display:none;
                                            }}
                                            #outlook a {{
                                              padding:0;
                                            }}
                                            span.MsoHyperlink,
                                            span.MsoHyperlinkFollowed {{
                                              color:inherit;
                                              mso-style-priority:99;
                                            }}
                                            a.es-button {{
                                              mso-style-priority:100!important;
                                              text-decoration:none!important;
                                            }}
                                            a[x-apple-data-detectors],
                                            #MessageViewBody a {{
                                              color:inherit!important;
                                              text-decoration:none!important;
                                              font-size:inherit!important;
                                              font-family:inherit!important;
                                              font-weight:inherit!important;
                                              line-height:inherit!important;
                                            }}
                                            .es-desk-hidden {{
                                              display:none;
                                              float:left;
                                              overflow:hidden;
                                              width:0;
                                              max-height:0;
                                              line-height:0;
                                              mso-hide:all;
                                            }}
                                            .es-button-border:hover {{
                                              border-color:#3d5ca3 #3d5ca3 #3d5ca3 #3d5ca3!important;
                                              background:#ffffff!important;
                                            }}
                                            .es-button-border:hover a.es-button,
                                            .es-button-border:hover button.es-button {{
                                              background:#ffffff!important;
                                            }}
                                            .es-button-border:hover a.es-button {{
                                              background:#ffffff!important;
                                              border-color:#ffffff!important;
                                            }}
                                            @media only screen and (max-width:600px) {{.es-m-p20b {{ padding-bottom:20px!important }} .es-m-p0l {{ padding-left:0px!important }} .es-p-default {{ }} *[class=""gmail-fix""] {{ display:none!important }} p, a {{ line-height:150%!important }} h1, h1 a {{ line-height:120%!important }} h2, h2 a {{ line-height:120%!important }} h3, h3 a {{ line-height:120%!important }} h4, h4 a {{ line-height:120%!important }} h5, h5 a {{ line-height:120%!important }} h6, h6 a {{ line-height:120%!important }} .es-header-body p {{ }} .es-content-body p {{ }} .es-footer-body p {{ }} .es-infoblock p {{ }} h1 {{ font-size:20px!important; text-align:center; line-height:120%!important }} h2 {{ font-size:16px!important; text-align:left; line-height:120%!important }} h3 {{ font-size:20px!important; text-align:center; line-height:120%!important }} h4 {{ font-size:24px!important; text-align:left }} h5 {{ font-size:20px!important; text-align:left }} h6 {{ font-size:16px!important; text-align:left }} .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a {{ font-size:20px!important }} .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a {{ font-size:16px!important }} .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a {{ font-size:20px!important }} .es-header-body h4 a, .es-content-body h4 a, .es-footer-body h4 a {{ font-size:24px!important }} .es-header-body h5 a, .es-content-body h5 a, .es-footer-body h5 a {{ font-size:20px!important }} .es-header-body h6 a, .es-content-body h6 a, .es-footer-body h6 a {{ font-size:16px!important }} .es-menu td a {{ font-size:14px!important }} .es-header-body p, .es-header-body a {{ font-size:10px!important }} .es-content-body p, .es-content-body a {{ font-size:16px!important }} .es-footer-body p, .es-footer-body a {{ font-size:12px!important }} .es-infoblock p, .es-infoblock a {{ font-size:12px!important }} .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3, .es-m-txt-c h4, .es-m-txt-c h5, .es-m-txt-c h6 {{ text-align:center!important }} .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3, .es-m-txt-r h4, .es-m-txt-r h5, .es-m-txt-r h6 {{ text-align:right!important }} .es-m-txt-j, .es-m-txt-j h1, .es-m-txt-j h2, .es-m-txt-j h3, .es-m-txt-j h4, .es-m-txt-j h5, .es-m-txt-j h6 {{ text-align:justify!important }} .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3, .es-m-txt-l h4, .es-m-txt-l h5, .es-m-txt-l h6 {{ text-align:left!important }} .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img {{ display:inline!important }} .es-m-txt-r .rollover:hover .rollover-second, .es-m-txt-c .rollover:hover .rollover-second, .es-m-txt-l .rollover:hover .rollover-second {{ display:inline!important }} .es-m-txt-r .rollover span, .es-m-txt-c .rollover span, .es-m-txt-l .rollover span {{ line-height:0!important; font-size:0!important; display:block }} .es-spacer {{ display:inline-table }} a.es-button, button.es-button {{ font-size:14px!important; padding:10px 20px 10px 20px!important; line-height:120%!important }} a.es-button, button.es-button, .es-button-border {{ display:inline-block!important }} .es-m-fw, .es-m-fw.es-fw, .es-m-fw .es-button {{ display:block!important }} .es-m-il, .es-m-il .es-button, .es-social, .es-social td, .es-menu {{ display:inline-block!important }} .es-adaptive table, .es-left, .es-right {{ width:100%!important }} .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header {{ width:100%!important; max-width:600px!important }} .adapt-img {{ width:100%!important; height:auto!important }} .es-mobile-hidden, .es-hidden {{ display:none!important }} .es-desk-hidden {{ width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important }} tr.es-desk-hidden {{ display:table-row!important }} table.es-desk-hidden {{ display:table!important }} td.es-desk-menu-hidden {{ display:table-cell!important }} .es-menu td {{ width:1%!important }} table.es-table-not-adapt, .esd-block-html table {{ width:auto!important }} .h-auto {{ height:auto!important }} h2 a {{ text-align:left }} a.es-button {{ border-left-width:0px!important; border-right-width:0px!important }} }}
                                            @media screen and (max-width:384px) {{.mail-message-content {{ width:414px!important }} }}
                                            </style>
                                             </head>
                                             <body class=""body"" style=""width:100%;height:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0"">
                                              <div dir=""ltr"" class=""es-wrapper-color"" lang=""en"" style=""background-color:#FAFAFA"">
                                               <!--[if gte mso 9]>
			                                            <v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				                                            <v:fill type=""tile"" color=""#fafafa""></v:fill>
			                                            </v:background>
		                                            <![endif]-->
                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" class=""es-wrapper"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FAFAFA"">
                                                 <tr style=""border-collapse:collapse"">
                                                  <td valign=""top"" style=""padding:0;Margin:0"">
                                                   <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td align=""center"" class=""es-adaptive"" style=""padding:0;Margin:0"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""#ffffff"" align=""center"" class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" data-custom-paddings=""true"" style=""padding:10px;Margin:0"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:580px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" class=""es-infoblock"" style=""padding:0;Margin:0""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;letter-spacing:0;color:#CCCCCC;font-size:16px"">&nbsp;Wo Ho !! did you forget your password ??&nbsp;</p></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellpadding=""0"" cellspacing=""0"" align=""center"" class=""es-header"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td align=""center"" class=""es-adaptive"" style=""padding:0;Margin:0"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""#3d5ca3"" align=""center"" class=""es-header-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#3D5CA3;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td bgcolor=""#3d5ca3"" align=""left"" style=""Margin:0;padding-top:20px;padding-right:20px;padding-bottom:20px;padding-left:20px;background-color:#3D5CA3"" data-custom-paddings=""true"">
                                                           <!--[if mso]><table style=""width:560px"" cellpadding=""0"" 
                                                                    cellspacing=""0""><tr><td style=""width:270px"" valign=""top""><![endif]-->
                                                           <table cellspacing=""0"" cellpadding=""0"" align=""left"" class=""es-left"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td align=""left"" class=""es-m-p20b"" style=""padding:0;Margin:0;width:270px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" class=""es-m-p0l es-m-txt-c"" style=""padding:0;Margin:0;font-size:0""><a href=""https://viewstripo.email"" target=""_blank"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:14px;color:#1376C8""><img src=""https://frvfdrb.stripocdn.email/content/guids/CABINET_66498ea076b5d00c6f9553055acdb37a/images/12051527590691841.png"" alt="""" width=""183"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></a></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table> 
                                                           <!--[if mso]></td><td style=""width:20px""></td><td style=""width:270px"" valign=""top""><![endif]-->
                                                           <table cellspacing=""0"" cellpadding=""0"" align=""right"" class=""es-right"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td align=""left"" style=""padding:0;Margin:0;width:270px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""right"" class=""es-m-txt-c"" style=""padding:0;Margin:0;padding-top:10px""><span class=""es-button-border"" style=""border-style:solid;border-color:#3D5CA3;background:#FFFFFF;border-width:2px;display:inline-block;border-radius:10px;width:auto""><a href=""https://viewstripo.email/"" target=""_blank"" class=""es-button"" style=""mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:14px;color:#3D5CA3;padding:10px 20px 10px 20px;display:inline-block;background:#FFFFFF;border-radius:10px;font-weight:bold;font-style:normal;line-height:16.8px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #FFFFFF"">Try free class</a></span></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table> 
                                                           <!--[if mso]></td></tr></table><![endif]--></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td bgcolor=""#fafafa"" align=""center"" style=""padding:0;Margin:0;background-color:#FAFAFA"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""#ffffff"" align=""center"" class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td bgcolor=""transparent"" align=""left"" style=""padding:0;Margin:0;padding-right:20px;padding-left:20px;padding-top:40px;background-color:transparent;background-position:left top"" data-custom-paddings=""true"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:560px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-position:left top"" role=""presentation"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0;padding-top:5px;padding-bottom:5px;font-size:0""><img src=""https://frvfdrb.stripocdn.email/content/guids/CABINET_dd354a98a803b60e2f0411e893c82f56/images/23891556799905703.png"" alt="""" width=""175"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px""><h1 style=""Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:20px;font-style:normal;font-weight:normal;line-height:24px;color:#333333""><strong>FORGOT YOUR </strong></h1><h1 style=""Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:20px;font-style:normal;font-weight:normal;line-height:24px;color:#333333""><strong>&nbsp;PASSWORD?</strong></h1></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" style=""padding:0;Margin:0;padding-right:40px;padding-left:40px""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;letter-spacing:0;color:#666666;font-size:16px;text-align:center"">HI,&nbsp;%FIRSTNAME|% %LASTNAME|%</p></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" style=""padding:0;Margin:0;padding-left:40px;padding-right:35px""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;letter-spacing:0;color:#666666;font-size:16px;text-align:center"">There was a request to change your password!</p></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0;padding-right:40px;padding-left:40px;padding-top:25px""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;letter-spacing:0;color:#666666;font-size:16px"">If did not make this request, just ignore this email. Otherwise, please click the button below to change your password:</p></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""Margin:0;padding-top:40px;padding-right:10px;padding-bottom:40px;padding-left:10px""><span class=""es-button-border"" style=""border-style:solid;border-color:#3D5CA3;background:#FFFFFF;border-width:2px;display:inline-block;border-radius:10px;width:auto""><a href=""http://localhost:4200/reset?email={email}&code{emailToken}"" target=""_blank"" class=""es-button"" style=""mso-style-priority:100 !important;text-decoration:none !important;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:14px;color:#3D5CA3;padding:10px 20px 10px 20px;display:inline-block;background:#FFFFFF;border-radius:10px;font-weight:bold;font-style:normal;line-height:16.8px;width:auto;text-align:center;letter-spacing:0;mso-padding-alt:0;mso-border-alt:10px solid #FFFFFF"">RESET PASSWORD</a></span></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" style=""padding:0;Margin:0;padding-top:20px;padding-right:10px;padding-left:10px;background-position:center center"" data-custom-paddings=""true"">
                                                           <!--[if mso]><table style=""width:580px"" cellpadding=""0"" cellspacing=""0""><tr><td style=""width:199px"" valign=""top""><![endif]-->
                                                           <table cellspacing=""0"" cellpadding=""0"" align=""left"" class=""es-left"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td align=""left"" style=""padding:0;Margin:0;width:199px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-position:center center"" role=""presentation"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""right"" style=""padding:0;Margin:0;padding-top:15px""><p class=""es-m-txt-c"" style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:24px;letter-spacing:0;color:#666666;font-size:16px""><strong>Follow us:</strong></p></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table> 
                                                           <!--[if mso]></td><td style=""width:20px""></td><td style=""width:361px"" valign=""top""><![endif]-->
                                                           <table cellspacing=""0"" cellpadding=""0"" align=""right"" class=""es-right"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td align=""left"" style=""padding:0;Margin:0;width:361px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-position:center center"" role=""presentation"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" class=""es-m-txt-c"" style=""padding:0;Margin:0;padding-top:10px;padding-bottom:5px;font-size:0"">
                                                                   <table cellspacing=""0"" cellpadding=""0"" class=""es-table-not-adapt es-social"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                     <tr style=""border-collapse:collapse"">
                                                                      <td valign=""top"" align=""center"" style=""padding:0;Margin:0;padding-right:10px""><img src=""https://frvfdrb.stripocdn.email/content/assets/img/social-icons/rounded-gray/facebook-rounded-gray.png"" alt=""Fb"" title=""Facebook"" width=""32"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                      <td valign=""top"" align=""center"" style=""padding:0;Margin:0;padding-right:10px""><img src=""https://frvfdrb.stripocdn.email/content/assets/img/social-icons/rounded-gray/x-rounded-gray.png"" alt=""X"" title=""X"" width=""32"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                      <td valign=""top"" align=""center"" style=""padding:0;Margin:0;padding-right:10px""><img src=""https://frvfdrb.stripocdn.email/content/assets/img/social-icons/rounded-gray/instagram-rounded-gray.png"" alt=""Ig"" title=""Instagram"" width=""32"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                      <td valign=""top"" align=""center"" style=""padding:0;Margin:0;padding-right:10px""><img src=""https://frvfdrb.stripocdn.email/content/assets/img/social-icons/rounded-gray/youtube-rounded-gray.png"" alt=""Yt"" title=""Youtube"" width=""32"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                      <td valign=""top"" align=""center"" style=""padding:0;Margin:0;padding-right:10px""><img src=""https://frvfdrb.stripocdn.email/content/assets/img/social-icons/rounded-gray/linkedin-rounded-gray.png"" alt=""In"" title=""Linkedin"" width=""32"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></td>
                                                                     </tr>
                                                                   </table></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table> 
                                                           <!--[if mso]></td></tr></table><![endif]--></td>
                                                         </tr>
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" style=""Margin:0;padding-right:20px;padding-bottom:20px;padding-left:20px;padding-top:5px;background-position:left top"" data-custom-paddings=""true"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:560px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:21px;letter-spacing:0;color:#666666;font-size:14px"">Contact us: <a target=""_blank"" href=""tel:123456789"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:14px;color:#666666"">123456789</a> | <a target=""_blank"" href=""mailto:your@mail.com"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:14px;color:#666666"">your@mail.com</a></p></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-footer"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td bgcolor=""#fafafa"" align=""center"" style=""padding:0;Margin:0;background-color:#FAFAFA"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""#ffffff"" align=""center"" class=""es-footer-body"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td bgcolor=""#0b5394"" align=""left"" style=""Margin:0;padding-right:20px;padding-left:20px;padding-top:10px;padding-bottom:30px;background-color:#0B5394;background-position:left top"" data-custom-paddings=""true"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:560px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" style=""padding:0;Margin:0;padding-top:5px;padding-bottom:5px""><h2 style=""Margin:0;font-family:arial, 'helvetica neue', helvetica, sans-serif;mso-line-height-rule:exactly;letter-spacing:0;font-size:16px;font-style:normal;font-weight:normal;line-height:19.2px;color:#ffffff""><strong>Have quastions?</strong></h2></td>
                                                                 </tr>
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""left"" style=""padding:0;Margin:0;padding-bottom:5px""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:21px;letter-spacing:0;color:#ffffff;font-size:14px"">We are here to help, learn more about us <a target=""_blank"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:14px;color:#ffffff"" href="""">here</a></p><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:21px;letter-spacing:0;color:#ffffff;font-size:14px"">or <a target=""_blank"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:14px;color:#ffffff"" href="""">contact us</a><br></p></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td bgcolor=""#fafafa"" align=""center"" style=""padding:0;Margin:0;background-color:#FAFAFA"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""transparent"" align=""center"" class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" style=""padding:0;Margin:0;padding-top:15px;background-position:left top"" data-custom-paddings=""true"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:600px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0;padding-right:20px;padding-bottom:20px;padding-left:20px;font-size:0"">
                                                                   <table width=""100%"" height=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                     <tr style=""border-collapse:collapse"">
                                                                      <td style=""padding:0;Margin:0;border-bottom:1px solid #FAFAFA;background:none;height:1px;width:100%;margin:0px""></td>
                                                                     </tr>
                                                                   </table></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-footer"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important;background-color:transparent;background-repeat:repeat;background-position:center top"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td bgcolor=""#fafafa"" align=""center"" style=""padding:0;Margin:0;background-color:#FAFAFA"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""transparent"" align=""center"" class=""es-footer-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" data-custom-paddings=""true"" style=""Margin:0;padding-right:20px;padding-left:20px;padding-bottom:5px;padding-top:15px"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:560px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" style=""padding:0;Margin:0""><p style=""Margin:0;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;line-height:18px;letter-spacing:0;color:#666666;font-size:12px"">This daily newsletter was sent to info@name.com from company name because you subscribed. If you would not like to receive this email <a target=""_blank"" class=""unsubscribe"" style=""mso-line-height-rule:exactly;text-decoration:underline;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:12px;color:#333333"" href="""">unsubscribe here</a>.</p></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table>
                                                   <table cellspacing=""0"" cellpadding=""0"" align=""center"" class=""es-content"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:100%;table-layout:fixed !important"">
                                                     <tr style=""border-collapse:collapse"">
                                                      <td align=""center"" style=""padding:0;Margin:0"">
                                                       <table cellspacing=""0"" cellpadding=""0"" bgcolor=""#ffffff"" align=""center"" class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;width:600px"" role=""none"">
                                                         <tr style=""border-collapse:collapse"">
                                                          <td align=""left"" data-custom-paddings=""true"" style=""Margin:0;padding-right:20px;padding-left:20px;padding-bottom:30px;padding-top:30px"">
                                                           <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""none"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                             <tr style=""border-collapse:collapse"">
                                                              <td valign=""top"" align=""center"" style=""padding:0;Margin:0;width:560px"">
                                                               <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px"">
                                                                 <tr style=""border-collapse:collapse"">
                                                                  <td align=""center"" class=""es-infoblock made_with"" style=""padding:0;Margin:0;font-size:0""><a target=""_blank"" href=""https://viewstripo.email/?utm_source=templates&utm_medium=email&utm_campaign=education&utm_content=trigger_newsletter2"" style=""mso-line-height-rule:exactly;text-decoration:none;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:12px;color:#CCCCCC""><img src=""https://frvfdrb.stripocdn.email/content/guids/cab_pub_7cbbc409ec990f19c78c75bd1e06f215/images/78411525331495932.png"" alt="""" width=""125"" style=""display:block;font-size:14px;border:0;outline:none;text-decoration:none""></a></td>
                                                                 </tr>
                                                               </table></td>
                                                             </tr>
                                                           </table></td>
                                                         </tr>
                                                       </table></td>
                                                     </tr>
                                                   </table></td>
                                                 </tr>
                                               </table>
                                              </div>
                                             </body>
                                            </html>";
        }
    }
}