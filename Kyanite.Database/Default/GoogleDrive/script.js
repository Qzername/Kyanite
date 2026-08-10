//this is "API" for managing google Drive without need of OAuth or Service Accounts
//if you want to use google drive as your file server go to 
//https://script.google.com/
//and create script with this code as WebAPI

function doPost(e) {
  try {
    var jsonData = JSON.parse(e.postData.contents);
    var folderName = "Kyanite";
    var fileName = jsonData.filename; 
    var contentBase64 = jsonData.content; 

    var folders = DriveApp.getFoldersByName(folderName);

    var folder;

    if (folders.hasNext()) 
      folder = folders.next();
    else 
      folder = DriveApp.createFolder(folderName);

    var files = folder.getFilesByName(fileName);

    while (files.hasNext()) 
      files.next().setTrashed(true);
    
    var blob = Utilities.newBlob(Utilities.base64Decode(contentBase64), 'application/octet-stream', fileName);
    var newFile = folder.createFile(blob);

    return ContentService.createTextOutput(JSON.stringify({ status: "success", id: newFile.getId() }))
      .setMimeType(ContentService.MimeType.JSON);
  } 
  catch (err) {
    return ContentService.createTextOutput(JSON.stringify({ status: "error", message: err.toString() }))
      .setMimeType(ContentService.MimeType.JSON);
  }
}

function doGet(e) {
  try {
    var fileName = e.parameter.filename;
    var folderName = "Kyanite";
    
    var folders = DriveApp.getFoldersByName(folderName);

    if (!folders.hasNext()) 
      return ContentService.createTextOutput(JSON.stringify({ status: "error", message: "Directory does not exists" })).setMimeType(ContentService.MimeType.JSON);
    
    var folder = folders.next();
    var files = folder.getFilesByName(fileName);
    
    if (files.hasNext()) {
      var file = files.next();
      var base64Content = Utilities.base64Encode(file.getBlob().getBytes());
      
      return ContentService.createTextOutput(JSON.stringify({ 
        status: "success", 
        filename: fileName, 
        content: base64Content 
      })).setMimeType(ContentService.MimeType.JSON);
    } 
    else 
      return ContentService.createTextOutput(JSON.stringify({ status: "error", message: "File not found" })).setMimeType(ContentService.MimeType.JSON);
  } 
  catch (err) {
    return ContentService.createTextOutput(JSON.stringify({ status: "error", message: err.toString() })).setMimeType(ContentService.MimeType.JSON);
  }
}