
function PostController() {
    var ctrl = this
/*
    ctrl.title = "Hello World!"
    ctrl.content = "Lorem ipsum dolor"
    ctrl.score = 10*/

    ctrl.changeScore = function(x) {
        console.log(x);
        ctrl.updateScore({newScore: ctrl.post.score + x, post: ctrl.post});
    }
}

angular.module("postItem", [])
.component("postItem", {
    templateUrl: "post/post.template.html",
    controller: PostController,
    bindings: {
        post: "<",
        updateScore: "&",
    }
});