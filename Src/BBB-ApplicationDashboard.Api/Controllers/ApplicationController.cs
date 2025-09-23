using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class ApplicationController(
    IEmailService emailService,
    IApplicationService applicationService
) : CustomControllerBase
{
    [HttpPost("update-application-info")]
    public async Task<IActionResult> UpdateApplicationInfo(ApplicationInfo applicationInfo)
    {
        await applicationService.UpdateApplicationAsync(applicationInfo);
        return SucessResponse("Application updated successfully");
    }

    [HttpPost("submit-form")]
    public async Task<IActionResult> SubmitApplicationForm(SubmittedDataRequest request)
    {
        //! Create application in database
        var accreditationResponse = await applicationService.CreateApplicationAsync(request);
        return SucessResponse(
            data: new { applicationId = accreditationResponse.ApplicationId },
            message: accreditationResponse.IsDuplicate
                ? "Duplicate application detected. Email sent with existing application details."
                : "Application submitted successfully and confirmation email sent"
        );

        //TODO SEND TO SCV SERVER WITH SHAWKI-CHAN DATA
        return SucessResponse(data: new { applicationId = accreditationResponse.ApplicationId });
    }

    // [HttpPost("test-email")]
    // public async Task<IActionResult> TestEmail()
    // {
    //     try
    //     {
    //         const string recipientEmail = "omarsaleh12216@gmail.com";
    //         const string recipientName = "Omar Saleh";
    //         const string businessName = "Tech Solutions LLC";

    //         var testApplicationNumber =
    //             $"BBB-{DateTime.Now.Year}-ACC-TEST-{Random.Shared.Next(1000, 9999)}";

    //         var sampleRequest = new SubmittedDataRequest
    //         {
    //             PrimaryFirstName = recipientName,
    //             BusinessName = businessName,
    //             PrimaryBusinessEmail = recipientEmail,
    //         };

    //         var emailHtml = CreateEmailTemplate(
    //             sampleRequest,
    //             testApplicationNumber,
    //             $"http://localhost:7100/track/{Guid.NewGuid()}"
    //         );

    //         var emailMessage = new EmailMessage
    //         {
    //             To = recipientEmail,
    //             Subject = "BBB Email Template Test - " + testApplicationNumber,
    //             HtmlBody = emailHtml,
    //         };

    //         var emailSent = await emailService.SendAsync(emailMessage, CancellationToken.None);

    //         return emailSent
    //             ? SucessResponse(
    //                 data: new { testApplicationNumber },
    //                 message: "Test email sent successfully to " + recipientEmail
    //             )
    //             : ErrorResponse(data: "Failed to send test email");
    //     }
    //     catch (Exception ex)
    //     {
    //         return ErrorResponse(data: $"Test email failed: {ex.Message}");
    //     }
    // }

    private static string CreateEmailTemplate(
        SubmittedDataRequest request,
        string applicationNumber,
        string trackingUrl,
        bool isDuplicate = false
    )
    {
        var statusTitle = isDuplicate
            ? "Application Already Exists"
            : "Application Successfully Submitted";

        var statusMessage = isDuplicate
            ? $"Dear {request.PrimaryFirstName} {request.PrimaryLastName}, we found an existing application for {request.BusinessName}."
            : "Your BBB Accreditation application has been received and is now under review.";

        var statusText = isDuplicate ? "Previously Submitted" : "Under Review";
        var buttonText = isDuplicate ? "Track Your Existing Application" : "Track Your Application";
        var statusIcon = isDuplicate ? "⚠️" : "✓";

        var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

        return $$"""
<!DOCTYPE html>
<html
  lang="en"
  xmlns="http://www.w3.org/1999/xhtml"
  xmlns:v="urn:schemas-microsoft-com:vml"
  xmlns:o="urn:schemas-microsoft-com:office:office"
>
  <head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="x-apple-disable-message-reformatting" />
    <meta
      name="format-detection"
      content="telephone=no,address=no,email=no,date=no,url=no"
    />
    <meta name="color-scheme" content="light dark" />
    <meta name="supported-color-schemes" content="light dark" />
    <title>BBB Application Submitted</title>

    <!--[if mso]>
    <noscript>
        <xml>
            <o:OfficeDocumentSettings>
                <o:AllowPNG/>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
    </noscript>
    <![endif]-->

    <!--[if (gte mso 9)|(IE)]>
    <style type="text/css">
        .outlook-mobile-hidden { display: none !important; }
        table { border-collapse: collapse !important; }
        td { border-collapse: collapse !important; }
        .outlook-mobile-text { font-size: 16px !important; }
        .outlook-mobile-padding { padding: 16px !important; }
        .outlook-mobile-full-width { width: 100% !important; max-width: none !important; }
    </style>
    <![endif]-->

</head>

<body style="margin: 0; padding: 0; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; background-color: #fafafa; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
    <!-- Mobile Styles for Email Clients -->
    <!--[if !mso]><!-->
    <style type="text/css">
        @media only screen and (max-width: 600px) {
            .mobile-hidden { display: none !important; }
            .mobile-full-width { width: 100% !important; max-width: none !important; }
            .mobile-padding { padding: 16px !important; }
            .mobile-text { font-size: 16px !important; }
            .mobile-center { text-align: center !important; }
            .mobile-logo { width: 48px !important; height: 48px !important; }
            .mobile-title { font-size: 24px !important; }
            .mobile-subtitle { font-size: 14px !important; }
            .mobile-card-padding { padding: 20px !important; }
            .mobile-step-spacing { margin-bottom: 16px !important; }
            .mobile-button { padding: 12px 24px !important; font-size: 14px !important; }
            .mobile-step-number { width: 24px !important; height: 24px !important; font-size: 12px !important; }
            .mobile-app-id { padding: 4px 8px !important; font-size: 11px !important; max-width: 100% !important; }
        }
    </style>
    <!--<![endif]-->

    <!-- Preheader -->
    <div style="display: none; font-size: 1px; color: #fefefe; line-height: 1px; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;">
        {{statusTitle}} - {{(
                statusMessage.Length > 100
                    ? string.Concat(statusMessage.AsSpan(0, 100), "...")
                    : statusMessage
            )}}
    </div>

    <!-- Email Container -->
    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #fafafa; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
        <tr>
            <td align="center" style="padding: 40px 20px;">

                <!-- Main Container -->
                <table border="0" cellpadding="0" cellspacing="0" width="600" style="max-width: 600px; width: 100%; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">

                    <!-- Header -->
                    <tr>
                        <td align="center" style="padding: 0 0 40px 0;">
                            <div class="mobile-logo" style="width: 64px; height: 64px; margin-bottom: 24px; display: table; text-align: center;">
                                <img src="https://res.cloudinary.com/di9eivnck/image/upload/v1757700296/favicon_rbiz34.png" alt="BBB Logo" style="width: 100%; height: 100%; object-fit: contain; border: 0; height: auto; line-height: 100%; outline: none; text-decoration: none; -ms-interpolation-mode: bicubic;" />
        </div>
                            <h1 class="mobile-title" style="margin: 0 0 8px 0; font-size: 32px; font-weight: 700; color: #09090b; line-height: 1.2; letter-spacing: -0.02em; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                Better Business Bureau
                            </h1>
                            <p class="mobile-subtitle" style="margin: 0; font-size: 16px; color: #005f86; font-weight: 600; letter-spacing: 0.025em; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                Accreditation Services
                            </p>
            </td>
          </tr>
          
                    <!-- Main Card -->
                    <tr>
                        <td style="padding: 0 0 32px 0;">
                            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #ffffff; border: 1px solid #e4e4e7; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04); border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">

                                <!-- Status Header -->
                                <tr>
                                    <td align="center" class="mobile-card-padding" style="background: linear-gradient(135deg, #005f86 0%, #007bb5 100%); padding: 48px 32px;">
                                        <div style="width: 56px; height: 56px; background-color: rgba(255, 255, 255, 0.25); backdrop-filter: blur(10px); border-radius: 16px; display: table; text-align: center; margin: 0 auto 20px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);">
                                            <span style="display: table-cell; vertical-align: middle; font-size: 28px; color: #ffffff; text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3); font-weight: 700;">{{statusIcon}}</span>
                                        </div>
                                        <h2
                                          class="mobile-title"
                                          style="
                                            margin: 0 0 12px 0;
                                            font-size: 24px;
                                            font-weight: 700;
                                            color: #ffffff;
                                            line-height: 1.3;
                                            letter-spacing: -0.02em;
                                            text-shadow: none;
                                            font-family: -apple-system, BlinkMacSystemFont,
                                              'Segoe UI', Roboto, 'Helvetica Neue', Arial,
                                              sans-serif;
                                          "
                                        >
                                          {{statusTitle}}
                                        </h2>
                                        <p
                                          class="mobile-text"
                                          style="
                                            margin: 0;
                                            font-size: 16px;
                                            color: #ffffff;
                                            line-height: 1.5;
                                            max-width: 400px;
                                            text-shadow: none;
                                            font-family: -apple-system, BlinkMacSystemFont,
                                              'Segoe UI', Roboto, 'Helvetica Neue', Arial,
                                              sans-serif;
                                          "
                                        >
                                          {{statusMessage}}
                                        </p>
                  </td>
                </tr>

                                <!-- Content -->
                                <tr>
                                    <td class="mobile-card-padding" style="padding: 40px 32px; background-color: #ffffff;">

                                        <!-- Application Details -->
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%" role="presentation">
                                            <tr>
                                                <td style="padding: 0 0 24px 0;">
                                                    <h3 style="margin: 0 0 20px 0; font-size: 18px; font-weight: 600; color: #09090b; line-height: 1.4; border-bottom: 2px solid #005f86; padding-bottom: 8px; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                        Application Details
                                                    </h3>
                  </td>
                </tr>
              </table>
              
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%" style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden; margin-bottom: 32px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                            <tr style="border-bottom: 1px solid #e2e8f0;">
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 500; color: #64748b; width: 35%; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Application Number
                                                </td>
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 600; color: #0f172a; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                            <span
                              class="mobile-app-id"
                              style="
                                background-color: #f1f5f9;
                                border: 1px solid #cbd5e1;
                                padding: 6px 12px;
                                border-radius: 6px;
                                font-family: 'SF Mono', Monaco, 'Cascadia Code',
                                  'Roboto Mono', Consolas, monospace;
                                letter-spacing: 0.025em;
                                font-size: 12px;
                                display: inline-block;
                                word-break: break-all;
                              "
                            >
                              {{applicationNumber}}
                            </span>
                                                </td>
                                            </tr>
                                            <tr style="border-bottom: 1px solid #e2e8f0;">
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 500; color: #64748b; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Submission Date
                                                </td>
                          <td
                            style="
                              padding: 16px 20px;
                              font-size: 14px;
                              font-weight: 600;
                              color: #0f172a;
                              font-family: -apple-system, BlinkMacSystemFont,
                                'Segoe UI', Roboto, 'Helvetica Neue', Arial,
                                sans-serif;
                            "
                          >
                            {{currentDate}}
                          </td>
                                            </tr>
                                            <tr style="border-bottom: 1px solid #e2e8f0;">
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 500; color: #64748b; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Current Status
                        </td>
                          <td
                            style="
                              padding: 16px 20px;
                              font-size: 14px;
                              font-weight: 600;
                              color: #0f172a;
                              font-family: -apple-system, BlinkMacSystemFont,
                                'Segoe UI', Roboto, 'Helvetica Neue', Arial,
                                sans-serif;
                            "
                          >
                            <span
                              style="
                                background: linear-gradient(
                                  135deg,
                                  #005f86 0%,
                                  #007bb5 100%
                                );
                                color: #ffffff;
                                padding: 6px 12px;
                                border-radius: 20px;
                                font-size: 12px;
                                font-weight: 600;
                                letter-spacing: 0.025em;
                                display: inline-block;
                              "
                            >
                              {{statusText}}
                            </span>
                          </td>
                      </tr>
                      <tr>
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 500; color: #64748b; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Expected Response
                        </td>
                                                <td style="padding: 16px 20px; font-size: 14px; font-weight: 600; color: #0f172a; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    5-7 Business Days
                        </td>
                      </tr>
                    </table>
                    
                                        <!-- CTA Button -->
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-top: 1px solid #e2e8f0; padding-top: 32px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                            <tr>
                                                <td align="center" valign="middle" style="padding: 0; text-align: center; vertical-align: middle;">
                                                    <table border="0" cellpadding="0" cellspacing="0" style="margin: 0 auto; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                                        <tr>
                                                            <td align="center" valign="middle" style="text-align: center; vertical-align: middle;">
                                                                <!--[if mso]>
                                                                <v:roundrect xmlns:v="urn:schemas-microsoft-com:vml" xmlns:w="urn:schemas-microsoft-com:office:word" href="{{trackingUrl}}" style="height:48px;v-text-anchor:middle;width:240px;" arcsize="15%" stroke="f" fillcolor="#005f86">
                                                                    <w:anchorlock/>
                                                                    <center style="color:#ffffff;font-family:-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;font-size:16px;font-weight:600;">
                                                                        {{buttonText}}
                                                                    </center>
                                                                </v:roundrect>
                                                                <![endif]-->
                                                                <!--[if !mso]><!-- -->
                                  <a
                                    href="{{trackingUrl}}"
                                    target="_blank"
                                    class="mobile-button"
                                    style="
                                      background: linear-gradient(
                                        135deg,
                                        #005f86 0%,
                                        #007bb5 100%
                                      );
                                      color: #ffffff;
                                      text-decoration: none;
                                      padding: 16px 32px;
                                      border-radius: 8px;
                                      font-weight: 600;
                                      font-size: 16px;
                                      display: block;
                                      border: none;
                                      letter-spacing: 0.025em;
                                      box-shadow: 0 4px 12px
                                        rgba(0, 95, 134, 0.15);
                                      line-height: 1;
                                      font-family: -apple-system,
                                        BlinkMacSystemFont, 'Segoe UI', Roboto,
                                        'Helvetica Neue', Arial, sans-serif;
                                      text-align: center;
                                      margin: 16px auto;
                                      max-width: 240px;
                                    "
                                  >
                                    {{buttonText}}
                                  </a>
                                                                <!--<![endif]-->
                         </td>
                       </tr>
                     </table>
                         </td>
                       </tr>
                     </table>
                    
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          
          <!-- What Happens Next Section -->
          <tr>
              <td style="padding: 32px 0 24px 0;">
                  <table border="0" cellpadding="0" cellspacing="0" width="100%" style="border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                      <tr>
                          <td style="padding: 0 0 20px 0;">
                              <h3 style="margin: 0 0 20px 0; font-size: 18px; font-weight: 600; color: #09090b; line-height: 1.4; border-bottom: 2px solid #005f86; padding-bottom: 8px; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                  What Happens Next
                              </h3>
                          </td>
                      </tr>
                  </table>

                  <!-- Steps -->
                  <table border="0" cellpadding="0" cellspacing="0" width="100%" style="margin-bottom: 32px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                      <tr>
                          <td>
                              <!-- Step 1 -->
                              <table border="0" cellpadding="20" cellspacing="0" width="100%" class="mobile-step-spacing" style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; margin-bottom: 12px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                  <tr>
                                      <td style="width: 48px; vertical-align: top; padding-right: 16px;">
                                          <div class="mobile-step-number" style="width: 32px; height: 32px; background: linear-gradient(135deg, #005f86 0%, #007bb5 100%); border-radius: 8px; display: table; text-align: center;">
                                              <span style="display: table-cell; vertical-align: middle; color: #ffffff; font-size: 14px; font-weight: 700; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">1</span>
                                          </div>
                                      </td>
                                      <td style="vertical-align: top;">
                                          <h4 style="margin: 0 0 8px 0; font-size: 16px; font-weight: 600; color: #0f172a; line-height: 1.4; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              Initial Review
                                          </h4>
                                          <p style="margin: 0; font-size: 14px; color: #64748b; line-height: 1.5; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              Our team will review your application materials within 2-3 business days.
              </p>
            </td>
          </tr>
        </table>
        
                              <!-- Step 2 -->
                              <table border="0" cellpadding="20" cellspacing="0" width="100%" class="mobile-step-spacing" style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; margin-bottom: 12px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                  <tr>
                                      <td style="width: 48px; vertical-align: top; padding-right: 16px;">
                                          <div class="mobile-step-number" style="width: 32px; height: 32px; background: linear-gradient(135deg, #005f86 0%, #007bb5 100%); border-radius: 8px; display: table; text-align: center;">
                                              <span style="display: table-cell; vertical-align: middle; color: #ffffff; font-size: 14px; font-weight: 700; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">2</span>
                                          </div>
                                      </td>
                                      <td style="vertical-align: top;">
                                          <h4 style="margin: 0 0 8px 0; font-size: 16px; font-weight: 600; color: #0f172a; line-height: 1.4; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              Verification Process
                                          </h4>
                                          <p style="margin: 0; font-size: 14px; color: #64748b; line-height: 1.5; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              We may contact you for additional information or clarification.
                                          </p>
      </td>
    </tr>
  </table>

                              <!-- Step 3 -->
                              <table border="0" cellpadding="20" cellspacing="0" width="100%" style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation" class="mobile-step-spacing">
                                  <tr>
                                      <td style="width: 48px; vertical-align: top; padding-right: 16px;">
                                          <div class="mobile-step-number" style="width: 32px; height: 32px; background: linear-gradient(135deg, #005f86 0%, #007bb5 100%); border-radius: 8px; display: table; text-align: center;">
                                              <span style="display: table-cell; vertical-align: middle; color: #ffffff; font-size: 14px; font-weight: 700; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">3</span>
            </div>
                                      </td>
                                      <td style="vertical-align: top;">
                                          <h4 style="margin: 0 0 8px 0; font-size: 16px; font-weight: 600; color: #0f172a; line-height: 1.4; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              Final Decision
                                          </h4>
                                          <p style="margin: 0; font-size: 14px; color: #64748b; line-height: 1.5; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                              You will receive notification of our accreditation decision.
                                          </p>
                                      </td>
                                  </tr>
                              </table>
                          </td>
                      </tr>
                  </table>
            </td>
          </tr>
          
                    <!-- Contact Section -->
                    <tr>
                        <td style="padding: 24px 0;">
                            <table border="0" cellpadding="32" cellspacing="0" width="100%" class="mobile-card-padding" style="background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                <tr>
                  <td align="center">
                                        <h4 style="margin: 0 0 20px 0; font-size: 18px; font-weight: 600; color: #0f172a; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                            Questions? Contact Our Team
                                        </h4>

                                        <table border="0" cellpadding="0" cellspacing="0" style="margin-bottom: 16px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                            <tr>
                                                <td style="padding-right: 12px; font-size: 14px; font-weight: 600; color: #005f86; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Email:
                                                </td>
                                                <td>
                                                    <a href="mailto:accreditation@bbb.org" style="color: #005f86; text-decoration: none; font-weight: 500; font-size: 14px; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                        accreditation@bbb.org
                                                    </a>
                  </td>
                </tr>
              </table>
              
                                        <table border="0" cellpadding="0" cellspacing="0" style="margin-bottom: 16px; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt;" role="presentation">
                                            <tr>
                                                <td style="padding-right: 12px; font-size: 14px; font-weight: 600; color: #005f86; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                    Phone:
                        </td>
                                                <td>
                                                    <a href="tel:1-800-955-5100" style="color: #005f86; text-decoration: none; font-weight: 500; font-size: 14px; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                                        1-800-955-5100
                                                    </a>
                  </td>
                </tr>
              </table>
              
                                        <p style="margin: 0; font-size: 12px; color: #64748b; line-height: 1.5; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                            Office hours: Monday - Friday, 9:00 AM - 5:00 PM EST
                                        </p>
                  </td>
                </tr>
              </table>
                  </td>
                </tr>
                
          <!-- What Happens Next Section -->
          <tr>
          
          <!-- Footer -->
          <tr>
              <td align="center" style="padding-top: 32px; border-top: 1px solid #e2e8f0;">
                            <p style="margin: 0; font-size: 12px; color: #94a3b8; line-height: 1.5; text-align: center; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
                                © 2025 Better Business Bureau. All rights reserved.<br>
                                This email was sent regarding your BBB Accreditation application.
              </p>
            </td>
          </tr>
          
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
""";
    }
}
