# Azure Service Bus
* Azure Service Bus Features
    * Queue
* Client Library:
    * Azure.Messaging.ServiceBus [New]
    	* Source: https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/servicebus*
	* Microsoft.Azure.ServiceBus [Old]
* Important Classes
    
	| Class | Description |
	|-------|-------------|
	|ServiceBusAdministrationClient|Used to manage messaging entities within a service bus namespace|
	|ServiceBusClient|Top-level client to interact with all Service Bus entities|
	|ServiceBusSender|Used to send messages to queues and topics|
	|ServiceBusReceiver|User to receive messages from queues and subscriptions|
	|ServieBusProcessor|Provides an event-based model for receiving and processing messages|
	|ServiceBusMessage|Represents a message to be sent to the Service Bus|
	|ServiceBusReceivedMessage|Represents a message that has been received from Service Bus|


