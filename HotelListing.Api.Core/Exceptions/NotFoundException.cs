namespace HotelListing.API.Core.Exceptions;

public class NotFoundException : ApplicationException {
	public NotFoundException(string message, string key) : base(message) {

	}
}
