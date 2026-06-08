using OrbitalGuardian.Application.DTOs;
using OrbitalGuardian.Application.Interfaces;

namespace OrbitalGuardian.Application.Commands;

public record AcknowledgeAlertCommand(Guid AlertId) : ICommand<AlertResponse>;
