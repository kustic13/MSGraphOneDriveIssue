const http = require('http');
const functionUrl = 'http://localhost:7071/api/UploadAndEditFile';
const countOfRequests = 50;

for(let i = 0; i < countOfRequests; ++i) {
	var request = http.get(functionUrl, (res) => {
        if (res.statusCode != 200) {
            console.log('The exception was thrown')
        }
    });
}

