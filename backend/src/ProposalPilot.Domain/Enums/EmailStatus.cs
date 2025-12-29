namespace ProposalPilot.Domain.Enums;

public enum EmailStatus
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Opened = 3,
    Clicked = 4,
    Bounced = 5,
    SpamReported = 6,
    Unsubscribed = 7,
    Failed = 8
}
