﻿namespace SimpleCrudProject.Model;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<City>? Cities { get; set; }
}