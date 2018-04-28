#  Development Notes

Inclusion of 3 new projects

## Payvision.Domain
 - Defined all application's necessary features/activities to meet the business logic requirements
 - Defined the application's entities and model objects.

## Payvision.Service
 - Created to implement the necessary Domain features
 
## Payvision.Common
 - Created to host the IoC container

# Extra NuGet Packages

- FakeItEasy
- Serilog

To better isolate the responsibilities, the core business logic was placed on the Service project.
On the Domain project, you can find all the interfaces, and the Entities/Model objects for the application.

FakeItEasy was used to ensure the core business methods are being used when performing the main activities; Calc Bits, and Fraud Analysis.
Serilog was used to include logging for better issues tracking.

As mentioned on the second exercise, receiving a file path is not good, so, I've change the Method to accept a FraudRequest instead.
By passing the FraudRequest object as a parameter, you can either specify a file extension, or a file name.

- File Extension (For multiple processing)
  By using the file extension, the method Check(FraudRequest request) is going to look for all files inside the provided Directory based on the file informed file extension pattern.
   
- File Name (For single processing)
  By using the file name, the method Check(FraudRequest request) is going to look for a file with the provided name, inside the provided Directory
   
# Extra Unit Tests

### Class PositiveBitCouterTest
	- Count_ValidInput_MandatoryMethodsMustHaveBeenUsed	
	
### Class FraudRadarTests
	- Count_ValidInput_MandatoryMethodsMustHaveBeenUsed
	- CheckFraud_MustHaveReadOrdersFourtimes
	
### Class BitServiceTest
    - ValidateInput_Must_ThrowException_WithNegativeValues
	- ConvertToBitArray_Should_CreateTheRightArray
	- ReserveArray_MustReverseTheArray_AsExpected
	- ConvertToBitArray_Must_CountTheRightNumberOfBitOccorrences
	- GetBitPositions_Must_ReturnTheRightBitPositions
	- GetCountingBitsOutput_Must_ReturnTheRightResult
	
### Class FraudServiceTest
	- IsValidRequest_ShouldReturnFailedValidation_IfInvalid
	- IsValidRequest_ShouldSucceedValidation_IfCorrect
	- EnsureFilePathIsValid_ShouldThrowException_IfInvalid
	- EnsureFilePathIsValid_ShouldTNothrowException_IfValid
	- NormalizeEmailAddress_ShouldNormalizeEmailAsExpected
	- NormalizeStateAddress_ShouldNormalizeStateAsExpected
	- NormalizeStateAddress_ShouldNotNormalizeState_IfNotIncludedInKnownStates
	- NormalizeStreetAddress_ShouldNormalizeStreetAsExcpected
	- ReadOrders_ShouldReturnCorrespondingNUmberOfConvertedObjects
	- EnsureFieldIsNumeric_ShouldThrownException_IfFieldInvalid
	- EnsureFIeldIsFilled_MustThrowException_IfInvalid
	- EnsureOrderHasAllMandatoryFields_ShouldThrowException_IfFieldNumberIsCorrect
	- EnsureOrderFieldsAreValid_ShouldThrowExceptionIfOneOfTheFieldsAreMissing
	- LookForCreditCardFraudByAddress_ShouldFail_IfThereAreFrauds
	- LookForCreditCardFraudByAddress_ShouldFSucceed_IfThereAreNoFrauds
	- LookForCreditCardFraudByEmail_ShouldFail_IfThereAreFrauds
	- LookForCreditCardFraudByEmail_ShouldSucceed_IfThereAreNoFrauds		
	
During the refactoring and the development, I've tried to follow the Clean Code concept, so the methods, and variable's names should speak by themselves, without having to comment everything.
