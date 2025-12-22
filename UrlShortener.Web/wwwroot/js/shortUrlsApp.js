(function() {
    'use strict';
    
    angular.module('shortUrlsApp', [])
        .service('shortUrlsService', ['$http', function($http) {
            this.getShortUrls = function() {
                return $http.get('/api/ShortUrlsApi').then(function(response) {
                    return response.data;
                });
            };
            
            this.createShortUrl = function(fullUrl) {
                return $http.post('/api/ShortUrlsApi', { fullUrl: fullUrl });
            };
            
            this.deleteShortUrl = function(id) {
                return $http.delete('/api/ShortUrlsApi/' + id);
            };
        }])
        .controller('ShortUrlsController', ['$scope', 'shortUrlsService', function($scope, shortUrlsService) {
            $scope.urls = [];
            $scope.loading = true;
            $scope.error = null;
            $scope.showModal = false;
            $scope.newUrl = '';
            $scope.isSubmitting = false;
            $scope.isAuthenticated = window.isAuthenticated === true;
            $scope.isAdmin = window.isAdmin === true;
            $scope.currentUserId = window.currentUserId;
            
            $scope.loadUrls = function() {
                $scope.loading = true;
                shortUrlsService.getShortUrls()
                    .then(function(data) {
                        $scope.urls = data.shortUrls || [];
                        $scope.error = null;
                        $scope.loading = false;
                    })
                    .catch(function(err) {
                        $scope.error = err.data?.error || err.data?.message || err.message || 'Failed to load URLs';
                        $scope.loading = false;
                    });
            };
            
            $scope.addUrl = function() {
                var urlInput = document.getElementById('fullUrl');
                var urlValue = urlInput ? urlInput.value : ($scope.newUrl || '').trim();
                
                console.log('addUrl called, newUrl from scope:', $scope.newUrl);
                console.log('addUrl called, urlValue from input:', urlValue);
                
                if (!urlValue) {
                    $scope.error = 'Please enter a URL';
                    console.log('URL is empty');
                    return;
                }
                
                $scope.isSubmitting = true;
                $scope.error = null;
                
                console.log('Calling createShortUrl with:', urlValue);
                
                shortUrlsService.createShortUrl(urlValue)
                    .then(function(response) {
                        console.log('Success:', response);
                        $scope.newUrl = '';
                        if (urlInput) urlInput.value = '';
                        $scope.showModal = false;
                        $scope.isSubmitting = false;
                        $scope.loadUrls();
                    })
                    .catch(function(err) {
                        console.error('Error creating URL:', err);
                        var errorMessage = 'Failed to create short URL';
                        if (err.data) {
                            if (err.data.error) {
                                errorMessage = err.data.error;
                            } else if (err.data.message) {
                                errorMessage = err.data.message;
                            }
                        } else if (err.message) {
                            errorMessage = err.message;
                        }
                        $scope.error = errorMessage;
                        $scope.isSubmitting = false;
                    });
            };
            
            $scope.deleteUrl = function(id) {
                if (!confirm('Are you sure you want to delete this URL?')) {
                    return;
                }
                
                shortUrlsService.deleteShortUrl(id)
                    .then(function() {
                        $scope.loadUrls();
                    })
                    .catch(function(err) {
                        $scope.error = err.data?.error || err.data?.message || 'Failed to delete URL';
                    });
            };
            
            $scope.formatDate = function(dateString) {
                if (!dateString) return 'N/A';
                return dateString;
            };
            
            $scope.getShortUrl = function(code) {
                return window.location.origin + '/' + code;
            };
            
            $scope.getInfoUrl = function(id) {
                return '/ShortUrls/info/' + id;
            };
            
            $scope.canDelete = function(url) {
                return $scope.isAdmin || ($scope.isAuthenticated && url.userId === $scope.currentUserId);
            };
            
            $scope.openAddModal = function() {
                console.log('openAddModal called, isAuthenticated:', $scope.isAuthenticated);
                $scope.showModal = true;
                $scope.newUrl = '';
                $scope.error = null;
                console.log('showModal set to:', $scope.showModal);
            };
            
            $scope.closeModal = function() {
                $scope.showModal = false;
                $scope.newUrl = '';
                $scope.error = null;
                $scope.isSubmitting = false;
            };
            
            $scope.closeError = function() {
                $scope.error = null;
            };
            
            // Initial load
            $scope.loadUrls();
            setInterval(function() {
                $scope.loadUrls();
            }, 30000);
        }]);
})();
