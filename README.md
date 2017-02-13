# NSW-eTendering-API
A developer portal for the NSW eTendering search API.  This repository will provide users of the data with documentation, code examples, bug notifications and feature requests.

##About
NSW eTendering is an online tendering system used by NSW Government agencies to make available information about tendering opportunities.

The API makes all eTendering data (RFTs, Schemes, CANs, SONs, and PPs) available on demand in a standard machine-readable format.

The service is provided based on the [NSW Government API Standard](https://www.finance.nsw.gov.au/ict/resources/api-standard), and compliant with the [Open Contracting Data standard](https://github.com/open-contracting/standard).  

##API usage
The API is available through a number of URLs:
[Planned Procurement search](https://tenders.nsw.gov.au/?event=api.planning.search)

[Tender search (Request For Tender and Scheme)](https://tenders.nsw.gov.au/?event=api.tender.search)

[Contract search (Contract Notice)](https://tenders.nsw.gov.au/?event=api.contract.search&type=cnEvent)

[Contract search (Standing Offer Notice)](https://tenders.nsw.gov.au/?event=api.contract.search&type=sonEvent)

[Individual Planned Procurement view](https://tenders.nsw.gov.au/?event=api.planning.view)

[Individual tender view (Request For Tender and Scheme)](https://tenders.nsw.gov.au/?event=api.tender.view)

[Individual contract view (Contract Notice and Standing Offer Notice)](https://tenders.nsw.gov.au/?event=api.contract.view)

More information on parameters and response fields is available in the Wiki section of this repository.

 
##Schema
The API schema is available in the Schema folder of this repository.

##Examples
Code examples are available in the examples folder of this repository.

##Rate Limiting
Requests are automatically limited by IP address.  If too many requests are received in a short period of time, that IP will for a short time receive a "You have made too many requests" message.  This time may change without notice based on system load.

##Support
The NSW Procurement Service Centre will provide support for API users.