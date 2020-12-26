using System;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using ReactiveUI;

namespace BetterTTD.App.BL.Models
{
    public sealed class MapModel : ReactiveObject
    {
        private string? _name;
        private Landscape _landscape;
        private DateTime _startDate;
        private DateTime _currentDate;
        private long _seed;
        private int _width;
        private int _height;

        public MapModel(Map map)
        {
            var startDate = map.StartDate;
            var currentDate = map.CurrentDate;
            
            Name = map.Name;
            Landscape = map.Landscape;
            StartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            CurrentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
            Seed = map.Seed;
            Width = map.Width;
            Height = map.Height;
        }
        
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public Landscape Landscape
        {
            get => _landscape;
            set => this.RaiseAndSetIfChanged(ref _landscape, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set => this.RaiseAndSetIfChanged(ref _currentDate, value);
        }

        public long Seed
        {
            get => _seed;
            set => this.RaiseAndSetIfChanged(ref _seed, value);
        }

        public int Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        public int Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }
    }
}