﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Controlador;
using Dominio;

namespace Clinica
{
    public partial class PagAltaTurno : System.Web.UI.Page
    {
        public List<Especialidades> listaEspecialidades { get; set; }
        public List<Medicos> listaMedicos { get; set; }

        public ControladorEspecialidades ctrlEsp = new ControladorEspecialidades();
        public ControladorMedicos ctrlMedicos = new ControladorMedicos();
        public ControladorPacientes ctrlPacientes = new ControladorPacientes();
        protected void Page_Load(object sender, EventArgs e)
        {
            ControladorMedicos controladorMedicos = new ControladorMedicos();

            if (!IsPostBack)
            {
                listaMedicos = ctrlMedicos.listar();
                Session["listaMedicos"] = listaMedicos;

                listaEspecialidades = ctrlEsp.ListarEspecialidades();

                ddlEspecialidades.DataSource = listaEspecialidades;
                ddlEspecialidades.DataTextField = "Nombre";
                ddlEspecialidades.DataValueField = "ID";
                ddlEspecialidades.DataBind();
                
            }
        }

        protected void ddlEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(ddlEspecialidades.SelectedItem.Value);

            ddlMedicos.DataSource = ctrlMedicos.ListarPorEspecialidad(id);
            ddlMedicos.DataTextField = "Apellido";
            ddlMedicos.DataValueField = "ID";
            ddlMedicos.DataBind();
        }

        protected void ddlMedicos_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(ddlMedicos.SelectedItem.Value);

            ddlDias.DataSource = ctrlMedicos.ListarDias(id);
            ddlDias.DataBind();

            ddlHorarios.DataSource = ctrlMedicos.ListarHorarios(id);
            ddlHorarios.DataBind();
        }

        protected void buscarPaciente_Click(object sender, EventArgs e)
        {
            try
            {
                List<Pacientes> listaPacientes = ctrlPacientes.listar();
                List<Pacientes> listaFiltrada = listaPacientes.FindAll(x => x.DNI.Contains(DNI.Text));
                if(listaFiltrada.Count == 0)
                {
                    txtValidar.Text = "No existen registros para el DNI " + DNI.Text;
                    txtAlta.Text = "Registrar paciente";
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}