using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MonitorSistemaWPF
{
    public class Medida : INotifyPropertyChanged
    {
        private string _nombre;
        private double _valor;
        private double _valorMaximo;
        private string _formato = "{0:F2}";
        private string _unidad = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public Medida()
        {
        }

        public Medida(string nombre, string formato = "{0:F2}", string unidad = "")
        {
            _nombre = nombre;
            _formato = formato;
            _unidad = unidad;
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Valor
        {
            get => _valor;
            set
            {
                if (Math.Abs(_valor - value) > 0.0001 || _valor == 0)
                {
                    _valor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ValorTexto));
                }

                if (_valor > _valorMaximo)
                {
                    ValorMaximo = _valor;
                }
            }
        }

        public double ValorMaximo
        {
            get => _valorMaximo;
            set
            {
                if (Math.Abs(_valorMaximo - value) > 0.0001 || _valorMaximo == 0)
                {
                    _valorMaximo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ValorMaximoTexto));
                }
            }
        }

        public string Formato
        {
            get => _formato;
            set
            {
                if (_formato != value)
                {
                    _formato = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ValorTexto));
                    OnPropertyChanged(nameof(ValorMaximoTexto));
                }
            }
        }

        public string Unidad
        {
            get => _unidad;
            set
            {
                if (_unidad != value)
                {
                    _unidad = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ValorTexto));
                    OnPropertyChanged(nameof(ValorMaximoTexto));
                }
            }
        }

        public string ValorTexto
        {
            get
            {
                try
                {
                    string texto = string.IsNullOrEmpty(_formato) ? _valor.ToString("F2") : string.Format(_formato, _valor);
                    return string.IsNullOrEmpty(_unidad) ? texto : $"{texto} {_unidad}".Trim();
                }
                catch
                {
                    return _valor.ToString();
                }
            }
        }

        public string ValorMaximoTexto
        {
            get
            {
                try
                {
                    string texto = string.IsNullOrEmpty(_formato) ? _valorMaximo.ToString("F2") : string.Format(_formato, _valorMaximo);
                    return string.IsNullOrEmpty(_unidad) ? texto : $"{texto} {_unidad}".Trim();
                }
                catch
                {
                    return _valorMaximo.ToString();
                }
            }
        }

        public void ReiniciarMaximo()
        {
            ValorMaximo = 0;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Nombre}: {ValorTexto} (Máx: {ValorMaximoTexto})";
        }
    }
}
