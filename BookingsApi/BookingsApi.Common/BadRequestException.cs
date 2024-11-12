using System;

namespace BookingsApi.Common;

public class BadRequestException(string message) : Exception(message);