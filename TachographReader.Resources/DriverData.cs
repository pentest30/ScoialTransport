/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace DriverData	
{
	[XmlRoot(ElementName = "CardExtendedSerialNumber")]
	public class CardExtendedSerialNumber
	{
		[XmlAttribute(AttributeName = "Month")]
		public string Month { get; set; }
		[XmlAttribute(AttributeName = "Year")]
		public string Year { get; set; }
		[XmlAttribute(AttributeName = "Type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "ManufacturerCode")]
		public string ManufacturerCode { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "EmbedderIcAssemblerId")]
	public class EmbedderIcAssemblerId
	{
		[XmlElement(ElementName = "CountryCode")]
		public string CountryCode { get; set; }
		[XmlElement(ElementName = "ModuleEmbedder")]
		public string ModuleEmbedder { get; set; }
		[XmlElement(ElementName = "ManufacturerInformation")]
		public string ManufacturerInformation { get; set; }
	}

	[XmlRoot(ElementName = "CardIccIdentification")]
	public class CardIccIdentification
	{
		[XmlElement(ElementName = "ClockStop")]
		public string ClockStop { get; set; }
		[XmlElement(ElementName = "CardExtendedSerialNumber")]
		public CardExtendedSerialNumber CardExtendedSerialNumber { get; set; }
		[XmlElement(ElementName = "CardApprovalNumber")]
		public string CardApprovalNumber { get; set; }
		[XmlElement(ElementName = "CardPersonaliserId")]
		public string CardPersonaliserId { get; set; }
		[XmlElement(ElementName = "EmbedderIcAssemblerId")]
		public EmbedderIcAssemblerId EmbedderIcAssemblerId { get; set; }
		[XmlElement(ElementName = "IcIdentifier")]
		public string IcIdentifier { get; set; }
	}

	[XmlRoot(ElementName = "IcSerialNumber")]
	public class IcSerialNumber
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "IcManufacturingReferences")]
	public class IcManufacturingReferences
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "CardChipIdentification")]
	public class CardChipIdentification
	{
		[XmlElement(ElementName = "IcSerialNumber")]
		public IcSerialNumber IcSerialNumber { get; set; }
		[XmlElement(ElementName = "IcManufacturingReferences")]
		public IcManufacturingReferences IcManufacturingReferences { get; set; }
	}

	[XmlRoot(ElementName = "DriverCardApplicationIdentification")]
	public class DriverCardApplicationIdentification
	{
		[XmlElement(ElementName = "Type")]
		public string Type { get; set; }
		[XmlElement(ElementName = "Version")]
		public string Version { get; set; }
		[XmlElement(ElementName = "NoOfEventsPerType")]
		public string NoOfEventsPerType { get; set; }
		[XmlElement(ElementName = "NoOfFaultsPerType")]
		public string NoOfFaultsPerType { get; set; }
		[XmlElement(ElementName = "ActivityStructureLength")]
		public string ActivityStructureLength { get; set; }
		[XmlElement(ElementName = "NoOfCardVehicleRecords")]
		public string NoOfCardVehicleRecords { get; set; }
		[XmlElement(ElementName = "NoOfCardPlaceRecords")]
		public string NoOfCardPlaceRecords { get; set; }
	}

	[XmlRoot(ElementName = "CardNumber")]
	public class CardNumber
	{
		[XmlAttribute(AttributeName = "ReplacementIndex")]
		public string ReplacementIndex { get; set; }
		[XmlAttribute(AttributeName = "RenewalIndex")]
		public string RenewalIndex { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "CardIssueDate")]
	public class CardIssueDate
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardValidityBegin")]
	public class CardValidityBegin
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardExpiryDate")]
	public class CardExpiryDate
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardIdentification")]
	public class CardIdentification
	{
		[XmlElement(ElementName = "CardIssuingMemberState")]
		public string CardIssuingMemberState { get; set; }
		[XmlElement(ElementName = "CardNumber")]
		public CardNumber CardNumber { get; set; }
		[XmlElement(ElementName = "CardIssuingAuthorityName")]
		public string CardIssuingAuthorityName { get; set; }
		[XmlElement(ElementName = "CardIssueDate")]
		public CardIssueDate CardIssueDate { get; set; }
		[XmlElement(ElementName = "CardValidityBegin")]
		public CardValidityBegin CardValidityBegin { get; set; }
		[XmlElement(ElementName = "CardExpiryDate")]
		public CardExpiryDate CardExpiryDate { get; set; }
	}

	[XmlRoot(ElementName = "CardHolderBirthDate")]
	public class CardHolderBirthDate
	{
		[XmlAttribute(AttributeName = "Datef")]
		public string Datef { get; set; }
	}

	[XmlRoot(ElementName = "DriverCardHolderIdentification")]
	public class DriverCardHolderIdentification
	{
		[XmlElement(ElementName = "CardHolderSurname")]
		public string CardHolderSurname { get; set; }
		[XmlElement(ElementName = "CardHolderFirstNames")]
		public string CardHolderFirstNames { get; set; }
		[XmlElement(ElementName = "CardHolderBirthDate")]
		public CardHolderBirthDate CardHolderBirthDate { get; set; }
		[XmlElement(ElementName = "CardHolderPreferredLanguage")]
		public string CardHolderPreferredLanguage { get; set; }
	}

	[XmlRoot(ElementName = "Identification")]
	public class Identification
	{
		[XmlElement(ElementName = "CardIdentification")]
		public CardIdentification CardIdentification { get; set; }
		[XmlElement(ElementName = "DriverCardHolderIdentification")]
		public DriverCardHolderIdentification DriverCardHolderIdentification { get; set; }
	}

	[XmlRoot(ElementName = "EventBeginTime")]
	public class EventBeginTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "EventEndTime")]
	public class EventEndTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "VehicleRegistration")]
	public class VehicleRegistration
	{
		[XmlElement(ElementName = "VehicleRegistrationNation")]
		public string VehicleRegistrationNation { get; set; }
		[XmlElement(ElementName = "VehicleRegistrationNumber")]
		public string VehicleRegistrationNumber { get; set; }
	}

	[XmlRoot(ElementName = "CardEventRecord")]
	public class CardEventRecord
	{
		[XmlElement(ElementName = "EventType")]
		public string EventType { get; set; }
		[XmlElement(ElementName = "EventBeginTime")]
		public EventBeginTime EventBeginTime { get; set; }
		[XmlElement(ElementName = "EventEndTime")]
		public EventEndTime EventEndTime { get; set; }
		[XmlElement(ElementName = "VehicleRegistration")]
		public VehicleRegistration VehicleRegistration { get; set; }
	}

	[XmlRoot(ElementName = "CardEventRecordCollection")]
	public class CardEventRecordCollection
	{
		[XmlElement(ElementName = "CardEventRecord")]
		public List<CardEventRecord> CardEventRecord { get; set; }
	}

	[XmlRoot(ElementName = "CardEventRecords")]
	public class CardEventRecords
	{
		[XmlElement(ElementName = "CardEventRecordCollection")]
		public List<CardEventRecordCollection> CardEventRecordCollection { get; set; }
	}

	[XmlRoot(ElementName = "EventsData")]
	public class EventsData
	{
		[XmlElement(ElementName = "CardEventRecords")]
		public CardEventRecords CardEventRecords { get; set; }
	}

	[XmlRoot(ElementName = "FaultBeginTime")]
	public class FaultBeginTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "FaultEndTime")]
	public class FaultEndTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardFaultRecord")]
	public class CardFaultRecord
	{
		[XmlElement(ElementName = "FaultType")]
		public string FaultType { get; set; }
		[XmlElement(ElementName = "FaultBeginTime")]
		public FaultBeginTime FaultBeginTime { get; set; }
		[XmlElement(ElementName = "FaultEndTime")]
		public FaultEndTime FaultEndTime { get; set; }
		[XmlElement(ElementName = "VehicleRegistration")]
		public VehicleRegistration VehicleRegistration { get; set; }
	}

	[XmlRoot(ElementName = "CardFaultRecordCollection")]
	public class CardFaultRecordCollection
	{
		[XmlElement(ElementName = "CardFaultRecord")]
		public List<CardFaultRecord> CardFaultRecord { get; set; }
	}

	[XmlRoot(ElementName = "CardFaultRecords")]
	public class CardFaultRecords
	{
		[XmlElement(ElementName = "CardFaultRecordCollection")]
		public List<CardFaultRecordCollection> CardFaultRecordCollection { get; set; }
	}

	[XmlRoot(ElementName = "FaultsData")]
	public class FaultsData
	{
		[XmlElement(ElementName = "CardFaultRecords")]
		public CardFaultRecords CardFaultRecords { get; set; }
	}

	[XmlRoot(ElementName = "ActivityChangeInfo")]
	public class ActivityChangeInfo
	{
		[XmlAttribute(AttributeName = "FileOffset")]
		public string FileOffset { get; set; }
		[XmlAttribute(AttributeName = "Slot")]
		public string Slot { get; set; }
		[XmlAttribute(AttributeName = "Status")]
		public string Status { get; set; }
		[XmlAttribute(AttributeName = "Inserted")]
		public string Inserted { get; set; }
		[XmlAttribute(AttributeName = "Activity")]
		public string Activity { get; set; }
		[XmlAttribute(AttributeName = "Time")]
		public string Time { get; set; }
	}

	[XmlRoot(ElementName = "CardActivityDailyRecord")]
	public class CardActivityDailyRecord
	{
		[XmlElement(ElementName = "ActivityChangeInfo")]
		public List<ActivityChangeInfo> ActivityChangeInfo { get; set; }
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
		[XmlAttribute(AttributeName = "DailyPresenceCounter")]
		public string DailyPresenceCounter { get; set; }
		[XmlAttribute(AttributeName = "Distance")]
		public string Distance { get; set; }
	}

	[XmlRoot(ElementName = "CardDriverActivity")]
	public class CardDriverActivity
	{
		[XmlElement(ElementName = "CardActivityDailyRecord")]
		public List<CardActivityDailyRecord> CardActivityDailyRecord { get; set; }
		[XmlElement(ElementName = "DataBufferIsWrapped")]
		public string DataBufferIsWrapped { get; set; }
	}

	[XmlRoot(ElementName = "DriverActivityData")]
	public class DriverActivityData
	{
		[XmlElement(ElementName = "CardDriverActivity")]
		public CardDriverActivity CardDriverActivity { get; set; }
	}

	[XmlRoot(ElementName = "VehicleFirstUse")]
	public class VehicleFirstUse
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "VehicleLastUse")]
	public class VehicleLastUse
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardVehicleRecord")]
	public class CardVehicleRecord
	{
		[XmlElement(ElementName = "VehicleOdometerBegin")]
		public string VehicleOdometerBegin { get; set; }
		[XmlElement(ElementName = "VehicleOdometerEnd")]
		public string VehicleOdometerEnd { get; set; }
		[XmlElement(ElementName = "VehicleFirstUse")]
		public VehicleFirstUse VehicleFirstUse { get; set; }
		[XmlElement(ElementName = "VehicleLastUse")]
		public VehicleLastUse VehicleLastUse { get; set; }
		[XmlElement(ElementName = "VehicleRegistration")]
		public VehicleRegistration VehicleRegistration { get; set; }
		[XmlElement(ElementName = "VuDataBlockCounter")]
		public string VuDataBlockCounter { get; set; }
	}

	[XmlRoot(ElementName = "CardVehicleRecords")]
	public class CardVehicleRecords
	{
		[XmlElement(ElementName = "CardVehicleRecord")]
		public List<CardVehicleRecord> CardVehicleRecord { get; set; }
	}

	[XmlRoot(ElementName = "CardVehiclesUsed")]
	public class CardVehiclesUsed
	{
		[XmlElement(ElementName = "VehiclePointerNewestRecord")]
		public string VehiclePointerNewestRecord { get; set; }
		[XmlElement(ElementName = "CardVehicleRecords")]
		public CardVehicleRecords CardVehicleRecords { get; set; }
	}

	[XmlRoot(ElementName = "EntryTime")]
	public class EntryTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "DailyWorkPeriodCountry")]
	public class DailyWorkPeriodCountry
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "PlaceRecord")]
	public class PlaceRecord
	{
		[XmlElement(ElementName = "EntryTime")]
		public EntryTime EntryTime { get; set; }
		[XmlElement(ElementName = "EntryType")]
		public string EntryType { get; set; }
		[XmlElement(ElementName = "DailyWorkPeriodCountry")]
		public DailyWorkPeriodCountry DailyWorkPeriodCountry { get; set; }
		[XmlElement(ElementName = "DailyWorkPeriodRegion")]
		public string DailyWorkPeriodRegion { get; set; }
		[XmlElement(ElementName = "VehicleOdometerValue")]
		public string VehicleOdometerValue { get; set; }
	}

	[XmlRoot(ElementName = "PlaceRecords")]
	public class PlaceRecords
	{
		[XmlElement(ElementName = "PlaceRecord")]
		public List<PlaceRecord> PlaceRecord { get; set; }
	}

	[XmlRoot(ElementName = "CardPlaceDailyWorkPeriod")]
	public class CardPlaceDailyWorkPeriod
	{
		[XmlElement(ElementName = "PlacePointerNewestRecord")]
		public string PlacePointerNewestRecord { get; set; }
		[XmlElement(ElementName = "PlaceRecords")]
		public PlaceRecords PlaceRecords { get; set; }
	}

	[XmlRoot(ElementName = "ControlTime")]
	public class ControlTime
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "ControlCardNumber")]
	public class ControlCardNumber
	{
		[XmlAttribute(AttributeName = "Type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "IssuingMemberState")]
		public string IssuingMemberState { get; set; }
		[XmlAttribute(AttributeName = "ReplacementIndex")]
		public string ReplacementIndex { get; set; }
		[XmlAttribute(AttributeName = "RenewalIndex")]
		public string RenewalIndex { get; set; }
	}

	[XmlRoot(ElementName = "ControlVehicleRegistration")]
	public class ControlVehicleRegistration
	{
		[XmlElement(ElementName = "VehicleRegistrationNation")]
		public string VehicleRegistrationNation { get; set; }
		[XmlElement(ElementName = "VehicleRegistrationNumber")]
		public string VehicleRegistrationNumber { get; set; }
	}

	[XmlRoot(ElementName = "ControlDownloadPeriodBegin")]
	public class ControlDownloadPeriodBegin
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "ControlDownloadPeriodEnd")]
	public class ControlDownloadPeriodEnd
	{
		[XmlAttribute(AttributeName = "DateTime")]
		public string DateTime { get; set; }
	}

	[XmlRoot(ElementName = "CardControlActivityDataRecord")]
	public class CardControlActivityDataRecord
	{
		[XmlElement(ElementName = "ControlType")]
		public string ControlType { get; set; }
		[XmlElement(ElementName = "ControlTime")]
		public ControlTime ControlTime { get; set; }
		[XmlElement(ElementName = "ControlCardNumber")]
		public ControlCardNumber ControlCardNumber { get; set; }
		[XmlElement(ElementName = "ControlVehicleRegistration")]
		public ControlVehicleRegistration ControlVehicleRegistration { get; set; }
		[XmlElement(ElementName = "ControlDownloadPeriodBegin")]
		public ControlDownloadPeriodBegin ControlDownloadPeriodBegin { get; set; }
		[XmlElement(ElementName = "ControlDownloadPeriodEnd")]
		public ControlDownloadPeriodEnd ControlDownloadPeriodEnd { get; set; }
	}

	[XmlRoot(ElementName = "SpecificConditionRecord")]
	public class SpecificConditionRecord
	{
		[XmlElement(ElementName = "EntryTime")]
		public EntryTime EntryTime { get; set; }
		[XmlElement(ElementName = "SpecificConditionType")]
		public string SpecificConditionType { get; set; }
	}

	[XmlRoot(ElementName = "SpecificConditionRecords")]
	public class SpecificConditionRecords
	{
		[XmlElement(ElementName = "SpecificConditionRecord")]
		public List<SpecificConditionRecord> SpecificConditionRecord { get; set; }
	}

	[XmlRoot(ElementName = "SpecificConditions")]
	public class SpecificConditions
	{
		[XmlElement(ElementName = "SpecificConditionRecords")]
		public SpecificConditionRecords SpecificConditionRecords { get; set; }
	}

	[XmlRoot(ElementName = "Signature")]
	public class Signature
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "PublicKeyRemainder")]
	public class PublicKeyRemainder
	{
		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "Nation")]
	public class Nation
	{
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "CertificationAuthorityReference")]
	public class CertificationAuthorityReference
	{
		[XmlElement(ElementName = "Nation")]
		public Nation Nation { get; set; }
		[XmlElement(ElementName = "NationCode")]
		public string NationCode { get; set; }
		[XmlElement(ElementName = "SerialNumber")]
		public string SerialNumber { get; set; }
		[XmlElement(ElementName = "AdditionalInfo")]
		public string AdditionalInfo { get; set; }
		[XmlElement(ElementName = "CaIdentifier")]
		public string CaIdentifier { get; set; }
	}

	[XmlRoot(ElementName = "CardCertificate")]
	public class CardCertificate
	{
		[XmlElement(ElementName = "Signature")]
		public Signature Signature { get; set; }
		[XmlElement(ElementName = "PublicKeyRemainder")]
		public PublicKeyRemainder PublicKeyRemainder { get; set; }
		[XmlElement(ElementName = "CertificationAuthorityReference")]
		public CertificationAuthorityReference CertificationAuthorityReference { get; set; }
	}

	[XmlRoot(ElementName = "CACertificate")]
	public class CACertificate
	{
		[XmlElement(ElementName = "Signature")]
		public Signature Signature { get; set; }
		[XmlElement(ElementName = "PublicKeyRemainder")]
		public PublicKeyRemainder PublicKeyRemainder { get; set; }
		[XmlElement(ElementName = "CertificationAuthorityReference")]
		public CertificationAuthorityReference CertificationAuthorityReference { get; set; }
	}

	[XmlRoot(ElementName = "DriverData")]
	public class DriverData
	{
		[XmlElement(ElementName = "CardIccIdentification")]
		public CardIccIdentification CardIccIdentification { get; set; }
		[XmlElement(ElementName = "CardChipIdentification")]
		public CardChipIdentification CardChipIdentification { get; set; }
		[XmlElement(ElementName = "DriverCardApplicationIdentification")]
		public DriverCardApplicationIdentification DriverCardApplicationIdentification { get; set; }
		[XmlElement(ElementName = "Identification")]
		public Identification Identification { get; set; }
		[XmlElement(ElementName = "EventsData")]
		public EventsData EventsData { get; set; }
		[XmlElement(ElementName = "FaultsData")]
		public FaultsData FaultsData { get; set; }
		[XmlElement(ElementName = "DriverActivityData")]
		public DriverActivityData DriverActivityData { get; set; }
		[XmlElement(ElementName = "CardVehiclesUsed")]
		public CardVehiclesUsed CardVehiclesUsed { get; set; }
		[XmlElement(ElementName = "CardPlaceDailyWorkPeriod")]
		public CardPlaceDailyWorkPeriod CardPlaceDailyWorkPeriod { get; set; }
		[XmlElement(ElementName = "CardControlActivityDataRecord")]
		public CardControlActivityDataRecord CardControlActivityDataRecord { get; set; }
		[XmlElement(ElementName = "SpecificConditions")]
		public SpecificConditions SpecificConditions { get; set; }
		[XmlElement(ElementName = "CardCertificate")]
		public CardCertificate CardCertificate { get; set; }
		[XmlElement(ElementName = "CACertificate")]
		public CACertificate CACertificate { get; set; }
	}

}
