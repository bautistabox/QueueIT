#pragma checksum "C:\Users\alexb\RiderProjects\QueueIT\QueueIT\Views\Account\Profile.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0a7efa4652c8b7272f980c8ba0563ef80473c7eb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Profile), @"mvc.1.0.view", @"/Views/Account/Profile.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Account/Profile.cshtml", typeof(AspNetCore.Views_Account_Profile))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\alexb\RiderProjects\QueueIT\QueueIT\Views\_ViewImports.cshtml"
using QueueIT;

#line default
#line hidden
#line 2 "C:\Users\alexb\RiderProjects\QueueIT\QueueIT\Views\_ViewImports.cshtml"
using QueueIT.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0a7efa4652c8b7272f980c8ba0563ef80473c7eb", @"/Views/Account/Profile.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a2b9037ae191d3126dbedb9a3d664eaf95b9523a", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Profile : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "C:\Users\alexb\RiderProjects\QueueIT\QueueIT\Views\Account\Profile.cshtml"
  
    ViewBag.Title = "Profile";
    Layout = "_LoggedInLayout";

#line default
#line hidden
            BeginContext(72, 5109, true);
            WriteLiteral(@"
<br/><br/>
<div align=""center"">
    <div id=""team-profile-container"" class=""rounded shadow"">
        <div class=""container-row"">
            <h4 id=""users-name"" class=""primary-text display-6""></h4>
            <label id=""users-label"" for=""users-name"" class=""secondary-text""></label>
        </div>
        <br/>
        <nav class=""my-nav"">
            <div class=""nav nav-tabs justify-content-center"" id=""nav-tab"" role=""tablist"">
                <a class=""nav-item nav-link active"" id=""nav-profile-tab"" data-toggle=""tab""
                   href=""#nav-profile"" role=""tab"" aria-controls=""nav-profile"" aria-selected=""True"">Profile</a>
                <a class=""nav-item nav-link"" id=""nav-queues-tab"" data-toggle=""tab""
                   href=""#nav-queues"" role=""tab"" aria-controls=""nav-queues"" aria-selected=""False"">Queues</a>
                <a class=""nav-item nav-link"" id=""nav-settings-tab"" data-toggle=""tab""
                   href=""#nav-settings"" role=""tab"" aria-controls=""nav-settings"" aria-selected=""Fa");
            WriteLiteral(@"lse"">Settings</a>
            </div>
        </nav>
        <div class=""tab-content background-grey"" id=""nav-tabContent"">
            <div class=""tab-pane fade show active"" id=""nav-profile"" role=""tabpanel"" aria-labelledby=""nav-profile-tab"">
                <br/>
                <div style=""width: 75%;"">
                    <div align=""left"">
                        <h4 class=""primary-text display-8"">Teams</h4>
                        <table class=""table table-responsive-sm"">
                            <tbody>
                            <tr>
                                <td class=""primary-text"" style=""width: 20%;""><a href=""#"" class=""team-name-link""><h4 class=""display-8"">Team Name</h4></a></td>
                                <td class=""secondary-text"" style=""width: 80%;"">this is a description of a team. a team is a group of tasks that are organized into queues. queues are grouped by the team they belong too. a team is a group of people who want to work together to organize and complete their ");
            WriteLiteral(@"work load.</td>
                            </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class=""tab-pane fade show"" id=""nav-queues"" role=""tabpanel"" aria-labelledby=""nav-queues-tab"">
                <br/>
                <div style=""width: 75%"">
                    <div align=""left"">
                        <div class=""container-col"">
                            <div class=""container-row rounded shadow background-dark-grey"">
                                <div class=""card m-10"">
                                    <button class=""card-body home-card shadow"">TEST QUEUE</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class=""tab-pane fade show"" id=""nav-settings"" role=""tabpanel"" aria=""labelledby=nav-settings-tab"">
              ");
            WriteLiteral(@"  <br/>
                <div style=""width:40%"">
                    <div align=""left"">
                        <div class=""form-group"">
                            <h4 class=""display-6 primary-text"">Account Details</h4>
                            <label for=""new-f-name"" class=""secondary-text"">First Name</label>
                            <input id=""new-f-name"" class=""form-control modal-input""/>
                            <label for=""new-l-name"" class=""secondary-text"">Last Name</label>
                            <input id=""new-l-name"" class=""form-control modal-input""/>
                            <label for=""new-u-name"" class=""secondary-text"">Username</label>
                            <input id=""new-u-name"" class=""form-control modal-input""/>
                        </div>
                        <div align=""center""><button class=""my-btn my-btn-primary"">Save Account Details</button></div>
                        <div>
                            <h4 class=""display-6 primary-text"">Notificatio");
            WriteLiteral(@"ns</h4>
                        </div>
                        <div align=""center"">
                            <div class=""dropdown show"">
                                <a class=""btn btn-secondary dropdown-toggle"" href=""#"" role=""button"" id=""dd-note-setting"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                                    Notification Settings
                                </a>
                                <div class=""dropdown-menu background-dark-grey"" aria-labelledby=""dd-note-setting"">
                                    <a class=""dropdown-item"" href=""#"">Never</a>
                                    <a class=""dropdown-item"" href=""#"">Due Date</a>
                                    <a class=""dropdown-item"" href=""#"">All Activity</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
