var app = angular.module("myApp", ["postItem", "postList", "newPost"]);

// service to handle http of getting posts
app.factory("postService", ["$http", "$rootScope", function($http, $rootScope) {

    function Post(title, content, publish_date, tags) {
        this.title = title; // string
        this.content = content; // string
        this.publish_date = publish_date; // Date object
        this.score = 0;
        this.tags = tags;
    }

    let posts = [];

    function getPosts() {
        // get posts from server
        $http({
            method: "GET",
            url: "api/post",
        }).then(function successCallback(response) {
            posts = response.data;
            console.log(posts);
            $rootScope.$broadcast("posts:updated", posts); // tell everyone else that posts have changed
        });

        return posts;
    }


    function createPost(title, content, tags) {
        // add locally
        post = new Post(title, content, new Date(), tags);
        posts.push(post);

        // send post request
        $http({
            method: "POST",
            url: "api/post/",
            data: angular.toJson(post)
        }).then(function successCallback(response) {
            console.log(response);
        }, function failureCallback(reason) {
            console.log(reason);
        });

    }

    function updatePost(post, title, content, score) {
        // update locally
        post.title = title;
        post.content = content;
        post.score = score;

       console.log(angular.toJson(post));

        // send PUT request
        let id = post.postId;
        $http({
            method: "PUT",
            url: "api/post/" + id,
            data: angular.toJson(post)
        }).then(function successCallback(response) {
            console.log(response);
        }, function failureCallback() {
            console.log("Failed to update!");
        });
    }

    return {
        getPosts: getPosts,
        createPost: createPost,
        updatePost: updatePost
    }
}]);

app.factory("tagService", ["$http", "$rootScope", function ($http, $rootScope) {
    // let tags = ["sports", "politics", "memes", "movies", "tech", "trump", "america", "world"];
    function getTags() {
        return $http({
            method: "GET",
            url: "api/tag",
        }).then(function successCallback(response) {
            let tags = response.data;
            // console.log(tags);
            $rootScope.$broadcast("tags:updated", tags);

            return tags;
        })
    }

    return {
        getTags: getTags
    };
}]);

app.controller("AppController", ["postService", "tagService", "$scope", function(postService, tagService, $scope) {
    console.log(postService.posts);
    $scope.posts = postService.getPosts();

    // watch for changes in service posts
    $scope.$on("posts:updated", function (event, data) {
        $scope.posts = data;
        console.log($scope.posts);
    })

    // handle add post button
    $scope.addPostVisible = false;
    $scope.addPostButtonClass = "btn btn-primary";
    $scope.buttonText = "Add Post";
    $scope.showAddPost = function() {
        $scope.addPostVisible = !$scope.addPostVisible;
        if ($scope.addPostVisible) {
            $scope.buttonText = "Cancel"
            $scope.addPostButtonClass = "btn btn-danger";
        } else {
            $scope.buttonText = "Add Post";
            $scope.addPostButtonClass = "btn btn-primary";
        }
    }
    
    // expose postService functions
    $scope.createPost = function (title, content, tags) {

        // if this is done on the main page scope, hide the add post stuff
        $scope.showAddPost();
        // create the post
        postService.createPost(title, content, tags);
    }
    $scope.updatePost = postService.updatePost;

    // tags stuff
    tagService.getTags().then((tags) => {
        $scope.tags = tags;
    })

    // listen for tags updated
    $scope.$on("tags:updated", function (event, data) {
        $scope.tags = data;
    })
}])