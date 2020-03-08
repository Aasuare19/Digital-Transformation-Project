<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Project1C._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Algorithm Project</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous"/>
    <link rel="stylesheet" type="text/css" href="style.css"/>
</head>
<body>
 
<form class="container-fluid" runat="server">   
    <asp:SqlDataSource ID="sqlDataSource" runat="server"></asp:SqlDataSource>
    <div class="header">
        <h1>Digital Transformation</h1>
    </div>
    <div class="FilterSpace">
        <asp:Label runat="server" text="Filter: " CssClass="h5"></asp:Label>
        <asp:DropDownList CssClass="btn btn-light dropdown-toggle" ID="StateSelect" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="StateSelect_SelectedIndexChanged">
            <asp:ListItem>All States</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList class="btn btn-light dropdown-toggle" ID="SchoolSelect" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="SchoolSelect_SelectedIndexChanged">
            <asp:ListItem>All Schools</asp:ListItem>
        </asp:DropDownList>
    </div>

    <!--Table on the left lists course descriptions-->
    <div class="LeftSide">
        <asp:GridView runat="server" CssClass="LeftSide table table-striped" ID="tblCourse">
            <Columns>
                <asp:TemplateField>
                    <itemtemplate>
                        <asp:LinkButton CssClass="btnSelect" runat="server" OnClick="Unnamed_Click" CommandArgument='<%# Eval("CourseID")%>'>Select</asp:LinkButton>
                    </itemtemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div class="RightSide" id="idRightSide" runat="server">
        <!--Output-->
        <div class="card text-left bg-light ">
            <div class="card-header" runat="server" id="lblCourseNumName"></div>
            <div class="card-body text-left">
                    <h2 class="card-title" runat="server" id="lblUniversity"></h2>
                    <h5 class="card-text" runat="server" id="lblCollege"></h5>
                    <h6 class="card-text" runat="server" id="lblDept"></h6>
                    <h6 class="card-text" runat="server" id="lblTrack"></h6>
                    <h6 class="card-text" runat="server" id="lblCore"></h6>
                <br />
                    <h5 class="card-text" runat="server" id="lblDescription"></h5>
                <br />
                    <h6 class="card-text" runat="server" id="lblScore"></h6>
            </div>
            <div class="card-footer text-muted font-weight-light" runat="server" id="lblLink">
            </div>
        </div>
    </div>
</form>
<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>
</body>
    <script>
        $(document).ready(function () {
            $(".btnSelect").click(function () {
                $(".RightSide").show();
                $("#lblDescription").text("Loading, please wait");
            });
        });
    </script>
</html>
