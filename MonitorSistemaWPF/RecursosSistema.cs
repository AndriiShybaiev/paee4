using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using MonitorSistema;
using static MonitorSistema.LectorRecursosSistema;

namespace MonitorSistemaWPF
{
    public class RecursosSistema : INotifyPropertyChanged
    {
        private readonly LectorRecursosSistema _lector;
        private readonly DispatcherTimer _timer;

        private string _usuario;
        private string _fabricante;
        private string _modelo;
        private string _procesador;
        private string _discosLogicos;
        private bool _estaEjecutando;

        public event PropertyChangedEventHandler PropertyChanged;

        public Medida Cpu { get; }
        public Medida MemoriaFisica { get; }
        public Medida MemoriaVirtual { get; }

        public Medida DiscoLectura { get; }
        public Medida DiscoEscritura { get; }
        public Medida DiscoLecturaEscritura { get; }
        public ObservableCollection<Medida> Discos { get; }

        public Medida RedRecibidos { get; }
        public Medida RedEnviados { get; }
        public Medida RedRecibidosEnviados { get; }
        public ObservableCollection<Medida> Red { get; }

        public ICommand IniciarCommand { get; set; }
        public ICommand PararCommand { get; set; }
        public ICommand ReiniciarMaximosCommand { get; set; }

        public RecursosSistema()
        {
            _lector = new LectorRecursosSistema();

            // Initialize metadata
            CargarMetadatosSistema();

            // Initialize Medida instances
            Cpu = new Medida("CPU", "{0:F2}%");
            MemoriaFisica = new Medida("Física", "{0:F2}%");
            MemoriaVirtual = new Medida("Virtual", "{0:F2}%");

            DiscoLectura = new Medida("Lectura", "{0:N0} B/s");
            DiscoEscritura = new Medida("Escritura", "{0:N0} B/s");
            DiscoLecturaEscritura = new Medida("Lectura/Escritura", "{0:N0} B/s");

            Discos = new ObservableCollection<Medida>
            {
                DiscoLectura,
                DiscoEscritura,
                DiscoLecturaEscritura
            };

            RedRecibidos = new Medida("Datos recibidos", "{0:N0} B/s");
            RedEnviados = new Medida("Datos enviados", "{0:N0} B/s");
            RedRecibidosEnviados = new Medida("Total datos red", "{0:N0} B/s");

            Red = new ObservableCollection<Medida>
            {
                RedRecibidos,
                RedEnviados,
                RedRecibidosEnviados
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += ManejadorTimer;

            // Initialize commands using generalized RelayCommand
            IniciarCommand = new RelayCommand(Iniciar, () => !EstaEjecutando);
            PararCommand = new RelayCommand(Parar, () => EstaEjecutando);
            ReiniciarMaximosCommand = new RelayCommand(ReiniciarMaximos);

            // Initial poll
            ActualizarMetricas();
        }

        public string Usuario
        {
            get => _usuario;
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Fabricante
        {
            get => _fabricante;
            set
            {
                if (_fabricante != value)
                {
                    _fabricante = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Modelo
        {
            get => _modelo;
            set
            {
                if (_modelo != value)
                {
                    _modelo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Procesador
        {
            get => _procesador;
            set
            {
                if (_procesador != value)
                {
                    _procesador = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DiscosLogicos
        {
            get => _discosLogicos;
            set
            {
                if (_discosLogicos != value)
                {
                    _discosLogicos = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EstaEjecutando
        {
            get => _estaEjecutando;
            set
            {
                if (_estaEjecutando != value)
                {
                    _estaEjecutando = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TextoEstado));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string TextoEstado => _estaEjecutando ? "Leyendo..." : "Parado";

        public void Iniciar()
        {
            if (!_estaEjecutando)
            {
                _timer.Start();
                EstaEjecutando = true;
            }
        }

        public void Parar()
        {
            if (_estaEjecutando)
            {
                _timer.Stop();
                EstaEjecutando = false;
            }
        }

        public void ReiniciarMaximos()
        {
            Cpu.ReiniciarMaximo();
            MemoriaFisica.ReiniciarMaximo();
            MemoriaVirtual.ReiniciarMaximo();

            foreach (var disco in Discos)
            {
                disco.ReiniciarMaximo();
            }

            foreach (var red in Red)
            {
                red.ReiniciarMaximo();
            }
        }

        private void CargarMetadatosSistema()
        {
            try
            {
                Usuario = _lector.getUsuario() ?? "Desconocido";
                Fabricante = _lector.getOrdenadorFabricante() ?? "Desconocido";
                Modelo = _lector.getOrdenadorModelo() ?? "Desconocido";
                var procesadores = _lector.getProcesadores();
                Procesador = procesadores != null && procesadores.Length > 0 ? procesadores[0] : "Desconocido";
                DiscosLogicos = _lector.getDiscosLogicos() ?? "Desconocido";
            }
            catch
            {
                Usuario = Usuario ?? "Desconocido";
                Fabricante = Fabricante ?? "Desconocido";
                Modelo = Modelo ?? "Desconocido";
                Procesador = Procesador ?? "Desconocido";
                DiscosLogicos = DiscosLogicos ?? "Desconocido";
            }
        }

        public void ActualizarMetricas()
        {
            try
            {
                double cpuVal = _lector.getValorCPU();
                if (cpuVal > 0 || Cpu.Valor == 0)
                {
                    Cpu.Valor = cpuVal;
                }

                double memFisica = _lector.getValorMemoriaFisicaPorcentaje();
                if (memFisica > 0 || MemoriaFisica.Valor == 0)
                {
                    MemoriaFisica.Valor = memFisica;
                }

                double memVirtual = _lector.getValorMemoriaVirtualPorcentaje();
                if (memVirtual > 0 || MemoriaVirtual.Valor == 0)
                {
                    MemoriaVirtual.Valor = memVirtual;
                }

                double dLectura = _lector.getDatosDisco(DiskData.Read);
                DiscoLectura.Valor = dLectura;

                double dEscritura = _lector.getDatosDisco(DiskData.Write);
                DiscoEscritura.Valor = dEscritura;

                // Total calculated from components of the same cycle
                DiscoLecturaEscritura.Valor = dLectura + dEscritura;

                double rRecibidos = _lector.getDatosRed(NetData.Received);
                RedRecibidos.Valor = rRecibidos;

                double rEnviados = _lector.getDatosRed(NetData.Sent);
                RedEnviados.Valor = rEnviados;

                // Total calculated from components of the same cycle
                RedRecibidosEnviados.Valor = rRecibidos + rEnviados;

                DiscosLogicos = _lector.getDiscosLogicos();
            }
            catch
            {
                // Suppress counter exceptions during transition
            }
        }

        private void ManejadorTimer(object sender, EventArgs e)
        {
            ActualizarMetricas();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
