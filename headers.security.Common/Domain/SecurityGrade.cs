namespace headers.security.Common.Domain;

// TODO: this is probably not enough during individual grading, might need to
// change to an info/low/medium/high/critical system like we do in reports
public enum SecurityGrade
{
    Unknown = 0,
    F = 1,
    E = 2,
    D = 3,
    C = 4,
    B = 5,
    A = 6,
    NonInfluencing = 7,
}