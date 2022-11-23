﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MiMaster.Master" AutoEventWireup="true" CodeBehind="PagTurnos.aspx.cs" Inherits="Clinica.PagTurnos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="font-family: Georgia (serif); font-size: xx-large; text-align: center; font-weight: 600" class="mb-3">
        <p>TURNOS MÉDICOS</p>
    </div>
    <table class="table table-striped table-bordered">
        <thead class="table-dark">
            <tr>
                <th scope="col" style="text-align: center;">ID</th>
                <th scope="col" style="text-align: center;">Paciente</th>
                <th scope="col" style="text-align: center;">Médico</th>
                <th scope="col" style="text-align: center;">Especialidad</th>
                <th scope="col" style="text-align: center;">Fecha</th>
                <th scope="col" style="text-align: center;">Hora</th>
                <th scope="col" style="text-align: center;"></th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repRepetidor">
                <ItemTemplate>
                    <tr>
                        <th scope="row" style="text-align: center"><%#Eval("ID") %></th>
                        <td style="text-align: center"><%#Eval("Medico.Nombre") %>  <%#Eval("Medico.Apellido") %></td>
                        <td style="text-align: center"><%#Eval("Paciente.Nombre") %>  <%#Eval("Paciente.Apellido") %></td>
                        <td style="text-align: center"><%#Eval("Especialidad.Nombre") %></td>
                        <td style="text-align: center"><%#Eval("Fecha") %></td>
                        <td style="text-align: center"><%#Eval("HoraEntrada") %>hs</td>
                        <td>
                            <a href="PagAltaMedico.aspx?id=<%#Eval("ID") %>"><i class="fas fa-search-plus" style="color: black;"></i></a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</asp:Content>
