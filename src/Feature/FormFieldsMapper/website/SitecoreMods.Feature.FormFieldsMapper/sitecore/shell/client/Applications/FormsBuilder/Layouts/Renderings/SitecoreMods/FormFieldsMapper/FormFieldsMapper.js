////(function (Speak) {
////    Speak.component({
////        name: "FormFieldsMapper",
////        initialize: function (app) {
////            debugger;
////        },
////        getTheListOfComponents: function (app) {
////            var componentsList = '';
////            $(Sitecore.Speak.app.components).each(function () { componentsList += '<li>' + this.id + '</li>'; });
////            $('.sc-CustomComponent[data-sc-id="' + this.id + '"]').html('<ul>' + componentsList + '</ul>');
////        }
////    });

////})(Sitecore.Speak);

(function (Speak) {
    Speak.component({
        initialize: function () {
            debugger;
            this.defineProperty("FormFields", []);
        },
        setFormFields: function (formFields) {
            this.FormFields = formFields;
        }
    }, "FormFieldsMapper");
})(Sitecore.Speak);