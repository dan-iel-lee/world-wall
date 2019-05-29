
function PostListController() {
    console.log("apudfpqweoufb");
    var ctrl = this;

    ctrl.updateScore = function(newScore, post) {
        ctrl.updatePost({post: post, title: post.title, content: post.content, score: newScore});
    }
}

angular.module("postList", ["postItem"])
.component("postList", {
    templateUrl: "post_list/post_list.template.html",
    bindings: {
        posts: "<",
        reverse: "<",
        updatePost: "&",
    },
    controller: PostListController
})