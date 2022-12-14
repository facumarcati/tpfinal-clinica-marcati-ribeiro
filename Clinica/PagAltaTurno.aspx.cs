using System;
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
        public List<Pacientes> listaPacientes { get; set; }
        public List<Turnos> listaTurnos { get; set; }
        public bool ValidarDias { get; set; }
        public int IDMedico { get; set; }

        public ControladorEspecialidades ctrlEsp = new ControladorEspecialidades();
        public ControladorMedicos ctrlMedicos = new ControladorMedicos();
        public ControladorPacientes ctrlPacientes = new ControladorPacientes();
        public ControladorTurnos ctrlTurnos = new ControladorTurnos();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                Session.Add("error", "No hay ningún usuario logueado");
                Response.Redirect("PagError.aspx");
            }
            if (((Usuarios)Session["usuario"]).Tipo == TipoUsuario.MEDICO && Request.QueryString["id"] == null)
            {
                Session.Add("error", "No tenés permisos para ingresar a esta pantalla");
                Response.Redirect("PagError.aspx", false);
            }

            ControladorMedicos controladorMedicos = new ControladorMedicos();
            ValidarDias = true;

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
            if (!IsPostBack && Request.QueryString["id"] != null)
            {
                listaTurnos = ctrlTurnos.Listar(Request.QueryString["id"]);
                Turnos seleccionado = listaTurnos[0];

                txtDNI.Text = seleccionado.Paciente.DNI;
                txtDNI.Enabled = false;

                ddlEspecialidades.SelectedValue = seleccionado.Especialidad.ID.ToString();
                ddlEspecialidades.Enabled = false;

                ddlMedicos.DataSource = ctrlMedicos.listar(seleccionado.Medico.ID.ToString());
                ddlMedicos.DataTextField = "Apellido";
                ddlMedicos.DataValueField = "ID";
                ddlMedicos.DataBind();
                ddlMedicos.Enabled = false;

                repDias.DataSource = ctrlMedicos.ListarDias(int.Parse(seleccionado.Medico.ID.ToString()));
                repDias.DataBind();

                txtFecha.Text = seleccionado.Fecha.ToShortDateString();

                ddlHorarios.DataSource = ctrlMedicos.ListarHorarios(int.Parse(seleccionado.Medico.ID.ToString()));
                ddlHorarios.DataBind();

                ddlHorarios.SelectedValue = seleccionado.HoraEntrada.ToString();

                txtObservaciones.Text = seleccionado.Observaciones;
            }
            if (((Usuarios)Session["usuario"]).Tipo == TipoUsuario.MEDICO && Request.QueryString["id"] != null)
            {
                txtObservaciones.Enabled = true;
                txtDNI.Enabled = false;
                ddlEspecialidades.Enabled = false;
                ddlMedicos.Enabled = false;
                ddlHorarios.Enabled = false;
                calDias.Enabled = false;
            }
            else
            {
                txtObservaciones.Enabled = false;
            }
        }

        protected void ddlEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(ddlEspecialidades.SelectedItem.Value);

            ddlMedicos.DataSource = ctrlMedicos.ListarPorEspecialidad(id);
            ddlMedicos.DataTextField = "Apellido";
            ddlMedicos.DataValueField = "ID";
            ddlMedicos.DataBind();

            if (ddlMedicos.Items.Count != 0)
            {
                ddlMedicos_SelectedIndexChanged(sender, e);
            }
            if (ddlMedicos.Items.Count == 0)
            {
                ddlHorarios.Items.Clear();
            }
        }

        protected void ddlMedicos_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDMedico = int.Parse(ddlMedicos.SelectedItem.Value);

            repDias.DataSource = ctrlMedicos.ListarDias(IDMedico);
            repDias.DataBind();

            ddlHorarios.DataSource = ctrlMedicos.ListarHorarios(IDMedico);
            ddlHorarios.DataBind();

            if (repDias.Items.Count == 0)
            {
                ValidarDias = false;
            }
            else
            {
                ValidarDias = true;
            }
        }

        protected void buscarPaciente_Click(object sender, EventArgs e)
        {
            try
            {
                List<Pacientes> listaPacientes = ctrlPacientes.listar();
                List<Pacientes> listaFiltrada = listaPacientes.FindAll(x => x.DNI.Contains(txtDNI.Text));
                if (listaFiltrada.Count == 0)
                {
                    paciente.Visible = false;
                    seleccionar.Visible = false;
                    cancelar.Visible = false;

                    txtValidar.Visible = true;
                    txtValidar.Text = "No existen registros para el DNI " + txtDNI.Text;
                    txtAlta.Visible = true;
                    txtAlta.Text = "Registrar paciente";
                    btnAceptar.Enabled = false;
                }
                else
                {
                    txtValidar.Visible = false;
                    txtAlta.Visible = false;

                    //Sin funcionalidad aun 
                    paciente.DataSource = listaFiltrada;
                    paciente.DataBind();
                    paciente.Visible = true;
                    //Se puede crear un metodo onlick con el boton y que quede seleccionado el paciente para registrar
                    seleccionar.Visible = true;
                    cancelar.Visible = true;
                    btnAceptar.Enabled = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void cancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("PagTurnos.aspx", false);
        }

        protected void calDias_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (calDias.SelectedDate < calDias.TodaysDate)
                {
                    lblValidarDia.Text = "La fecha ya pasó. Seleccione otra";
                    lblTest.Text = " ";
                    txtFecha.Text = " ";
                    btnAceptar.Enabled = false;
                }
                else
                {
                    btnAceptar.Enabled = true;

                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Monday")
                    {
                        lblTest.Text = "Lunes";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Tuesday")
                    {
                        lblTest.Text = "Martes";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Wednesday")
                    {
                        lblTest.Text = "Miércoles";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Thursday")
                    {
                        lblTest.Text = "Jueves";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Friday")
                    {
                        lblTest.Text = "Viernes";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Saturday")
                    {
                        lblTest.Text = "Sábado";
                    }
                    if (calDias.SelectedDate.DayOfWeek.ToString() == "Sunday")
                    {
                        lblTest.Text = "Domingo";
                    }

                    if (ValidarDia(lblTest.Text))
                    {
                        btnAceptar.Enabled = true;
                    }
                    else
                    {
                        btnAceptar.Enabled = false;
                    }

                    lblValidarDia.Text = " ";

                    txtFecha.Text = calDias.SelectedDate.ToShortDateString();

                    IDMedico = int.Parse(ddlMedicos.SelectedValue.ToString());

                    List<int> horariosMedico = ctrlMedicos.ListarHorarios(IDMedico);
                    List<int> horariosEntradaNoDisponibles = ctrlTurnos.HorariosNoDisponibles(IDMedico, DateTime.Parse(calDias.SelectedDate.ToShortDateString()));

                    foreach (int horarios in horariosEntradaNoDisponibles)
                    {
                        horariosMedico.Remove(horarios);
                    }

                    ddlHorarios.DataSource = horariosMedico;
                    ddlHorarios.DataBind();
                }
            }
            catch (Exception)
            {
                Session.Add("error", "Hubo un problema al seleccionar la fecha");
            }
        }

        protected bool ValidarDia(string dia)
        {
            List<HorariosTrabajo> dias = new List<HorariosTrabajo>();

            IDMedico = int.Parse(ddlMedicos.SelectedItem.Value);

            dias = ctrlMedicos.ListarDias(IDMedico);

            foreach (HorariosTrabajo item in dias)
            {
                if (item.Dia.ToString() == dia)
                {
                    return true;
                }
            }

            return false;
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            Turnos turnos = new Turnos();
            listaPacientes = ctrlPacientes.listar();

            try
            {
                turnos.Medico = new Medicos();
                turnos.Medico.ID = int.Parse(ddlMedicos.SelectedItem.Value);

                turnos.Paciente = new Pacientes();
                foreach (Pacientes item in listaPacientes)
                {
                    if (item.DNI == txtDNI.Text)
                    {
                        turnos.Paciente.ID = item.ID;
                    }
                }

                turnos.Especialidad = new Especialidades();
                turnos.Especialidad.ID = int.Parse(ddlEspecialidades.SelectedItem.Value);

                turnos.HoraEntrada = int.Parse(ddlHorarios.SelectedValue.ToString());
                turnos.Fecha = DateTime.Parse(txtFecha.Text);
                turnos.Observaciones = txtObservaciones.Text;

                listaTurnos = ctrlTurnos.Listar();

                if (Request.QueryString["id"] != null)
                {
                    turnos.ID = int.Parse(Request.QueryString["id"]);
                    ctrlTurnos.ModificarTurno(turnos);
                }
                else
                {
                    foreach (Turnos item in listaTurnos)
                    {
                        if (item.Paciente.ID == turnos.Paciente.ID && item.Fecha == turnos.Fecha)
                        {
                            Session.Add("error", "Este paciente ya tiene un turno este día");
                            Response.Redirect("PagError.aspx", false);
                            return;
                        }
                    }
                }

                if(Request.QueryString["id"] == null)
                {
                    ctrlTurnos.AgregarTurno(turnos);
                }

                Response.Redirect("PagTurnos.aspx", false);
            }
            catch (Exception)
            {
                Session.Add("error", "No se pudo agregar el turno");
                Response.Redirect("PagError.aspx");
            }
        }
    }
}