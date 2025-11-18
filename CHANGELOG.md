# Changelog  
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),  
and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.0.0] - 2025-11-18  
### Added  
- Initial release of **EasySerial**.  
- Simple, reliable serial communication handler for Unity.  
- Support for reading and writing data using `System.IO.Ports`.  
- Automatically scans and lists available COM ports.  
- Connection state events (OnConnected, OnDisconnected).  
- DataReceived event for incoming serial messages.  
- Sample scene: **Basic Serial Demo**.  
- Clean, modular API with SerialManager + SerialHandler.  

### Notes  
- Requires **API Compatibility Level = .NET Framework** to enable `System.IO.Ports` in Unity.  
