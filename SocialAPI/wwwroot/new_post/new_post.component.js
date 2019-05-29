
function NewPostController() {
    let ctrl = this

    ctrl.tagInputStr = ""

    ctrl.createPost = function(title, content) {
        console.log(title + " "  + content);
        ctrl.createPostBind({title: title, content: content, tags: ctrl.addedTags});
    }

    ctrl.onTagClicked = function (tagText) {
        ctrl.tagInputStr = tagText;
    }

    // handle tag creation
    ctrl.addedTags = []
    ctrl.addTag = function (tagText) {
        ctrl.addedTags.push(tagText);
    }
    ctrl.removeTag = function (tagText) {
        // find the tag
        for (var ii = 0; ii < ctrl.addedTags.length; ii++) {
            if (ctrl.addedTags[ii] === tagText) {
                ctrl.addedTags.splice(ii, 1); // remove one item starting from position ii
            }
        }
    }
}

let module = angular.module("newPost", [])

module.component("newPost", {
    templateUrl: "new_post/new_post.template.html",
    controller: NewPostController,
    bindings: {
        tags: '<',
        createPostBind: '&',
    }
});

module.directive("enterPressed", function () {

    function link(scope, element, attrs) {
        element.bind('keypress', function (event) {
            console.log('keypress' + ' ' + event.which);
            if (event.which == 13) {
                var action = attrs.enterPressed; // get function passed to directive
                scope.$apply(function () {
                    scope.$eval(action);
                });
                event.preventDefault();
            }
        })
    }

    return {
        restrict: 'A',
        link: link,
    }
})